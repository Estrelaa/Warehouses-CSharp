using ShipIt.Exceptions;
using ShipIt.Models.ApiModels;
using ShipIt.Models.DataModels;
using ShipIt.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ShipIt.Controllers
{
    public class InboundOrderController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IEmployeeRepository employeeRepository;
        private readonly ICompanyRepository companyRepository;
        private readonly IProductRepository productRepository;
        private readonly IStockRepository stockRepository;

        public InboundOrderController(IEmployeeRepository employeeRepository, ICompanyRepository companyRepository, IProductRepository productRepository, IStockRepository stockRepository)
        {
            this.employeeRepository = employeeRepository;
            this.stockRepository = stockRepository;
            this.companyRepository = companyRepository;
            this.productRepository = productRepository;
        }

        // GET api/status/{warehouseId}
        public InboundOrderResponse Get(int warehouseId)
        {

            log.Info("orderIn for warehouseId: " + warehouseId);

            // find the operations manager of the warehouse 
            var operationsManager = new Employee(employeeRepository.GetOperationsManager(warehouseId));

            log.Debug(String.Format("Found operations manager: {0}", operationsManager));

            //Gets all the stock accross all the warehouses 
            var LowStock = stockRepository.GetAllStockThatNeedRestocking(warehouseId);

            Dictionary<Company, List<InboundOrderLine>> orderlinesByCompany = new Dictionary<Company, List<InboundOrderLine>>();

            foreach (var stock in LowStock)
            {
                // get the company that makes the item
                Company company = MakeComp(stock);

                if (!orderlinesByCompany.ContainsKey(company))
                {
                    orderlinesByCompany.Add(company, new List<InboundOrderLine>());
                }

                // decide how many items to order 
                var orderQuantity = Math.Max(stock.LowerThreshold * 3 - stock.Held, stock.MinimumOrderQuantity);

                //put a new order in the dict
                orderlinesByCompany[company].Add(
                    new InboundOrderLine()
                    {
                        gtin = stock.ProductNumber,
                        name = stock.ProductName,
                        quantity = orderQuantity
                    });
            };

            log.Debug(String.Format("Constructed order lines: {0}", orderlinesByCompany));

            //order it
            var orderSegments = orderlinesByCompany.Select(ol => new OrderSegment()
            {
                OrderLines = ol.Value,
                Company = ol.Key
            });

            log.Info("Constructed inbound order");

            // return the order with right informaton
            return new InboundOrderResponse()
            {
                OperationsManager = operationsManager,
                WarehouseId = warehouseId,
                OrderSegments = orderSegments.ToList()
            };
        }

        private static Company MakeComp(RestockingDataModel stock)
        {
            Company company = new Company();

            company.Gcp = stock.CompanyID;
            company.Name = stock.CompanyName;
            company.Addr2 = stock.Addr2;
            company.Addr3 = stock.Addr3;
            company.Addr4 = stock.Addr4;

            company.PostalCode = stock.Postcode;
            company.City = stock.City;
            company.Tel = stock.Phone;
            company.Mail = stock.Mail;
            return company;
        }

        public void Post([FromBody]InboundManifestRequestModel requestModel)
        {
            log.Info("Processing manifest: " + requestModel);

            var gtins = new List<string>();

            foreach (var orderLine in requestModel.OrderLines)
            {
                if (gtins.Contains(orderLine.gtin))
                {
                    throw new ValidationException(String.Format("Manifest contains duplicate product gtin: {0}", orderLine.gtin));
                }
                gtins.Add(orderLine.gtin);
            }

            IEnumerable<ProductDataModel> productDataModels = productRepository.GetProductsByGtin(gtins);
            Dictionary<string, Product> products = productDataModels.ToDictionary(p => p.Gtin, p => new Product(p));

            log.Debug(String.Format("Retrieved products to verify manifest: {0}", products));

            var lineItems = new List<StockAlteration>();
            var errors = new List<string>();

            foreach (var orderLine in requestModel.OrderLines)
            {
                if (!products.ContainsKey(orderLine.gtin))
                {
                    errors.Add(String.Format("Unknown product gtin: {0}", orderLine.gtin));
                    continue;
                }

                Product product = products[orderLine.gtin];
                if (!product.Gcp.Equals(requestModel.Gcp))
                {
                    errors.Add(String.Format("Manifest GCP ({0}) doesn't match Product GCP ({1})",
                        requestModel.Gcp, product.Gcp));
                }
                else
                {
                    lineItems.Add(new StockAlteration(product.Id, orderLine.quantity));
                }
            }

            if (errors.Count() > 0)
            {
                log.Debug(String.Format("Found errors with inbound manifest: {0}", errors));
                throw new ValidationException(String.Format("Found inconsistencies in the inbound manifest: {0}", String.Join("; ", errors)));
            }

            log.Debug(String.Format("Increasing stock levels with manifest: {0}", requestModel));
            stockRepository.AddStock(requestModel.WarehouseId, lineItems);
            log.Info("Stock levels increased");
        }
            
    }
}
