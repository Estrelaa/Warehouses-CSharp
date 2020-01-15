using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls.WebParts;
using ShipIt.Exceptions;
using ShipIt.Models.ApiModels;
using ShipIt.Models.DataModels;
using ShipIt.Repositories;

namespace ShipIt.Controllers
{
    public class OutboundOrderController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IStockRepository stockRepository;
        private readonly IProductRepository productRepository;

        public OutboundOrderController(IStockRepository stockRepository, IProductRepository productRepository)
        {
            this.stockRepository = stockRepository;
            this.productRepository = productRepository;
        }

        public void Post([FromBody]OutboundOrderRequestModel request)
        {
            log.Info(String.Format("Processing outbound order: {0}", request));
         
            var lineItems = new List<StockAlteration>();
            var productIds = new List<int>();
            var gtins = AddGtins(request);

            Dictionary<string, Product> products = CreateProducts(gtins);
            foreach (var orderLine in request.OrderLines)
            {
                try
                {
                    var product = products[orderLine.gtin];
                    lineItems.Add(new StockAlteration(product.Id, orderLine.quantity));
                    productIds.Add(product.Id);
                }
                catch (KeyNotFoundException)
                {
                    throw new NoSuchEntityException(string.Join("; ", orderLine.gtin));
                }
            }

            var stock = stockRepository.GetStockByWarehouseAndProductIds(request.WarehouseId, productIds);
            var orderLines = request.OrderLines.ToList();
            List<string> errors = CheckStocks(lineItems, stock, orderLines, products);

            if (errors.Count > 0)
            {
                throw new InsufficientStockException(string.Join("; ", errors));
            }

            List<Truck> trucks = Truck(request);

            stockRepository.RemoveStock(request.WarehouseId, lineItems);
        }

        private static List<String> AddGtins(OutboundOrderRequestModel request)
        {
            var gtins = new List<String>();
            foreach (var orderLine in request.OrderLines)
            {
                if (gtins.Contains(orderLine.gtin))
                {
                    throw new ValidationException(String.Format("Outbound order request contains duplicate product gtin: {0}", orderLine.gtin));
                }
                gtins.Add(orderLine.gtin);
            }
            return gtins;
        }

        public Dictionary<string, Product> CreateProducts(List<string> gtins)
        {
            var productDataModels = productRepository.GetProductsByGtin(gtins);
            var products = productDataModels.ToDictionary(p => p.Gtin, p => new Product(p));
            return products;
        }

        private static List<string> CheckStocks(List<StockAlteration> lineItems, Dictionary<int, StockDataModel> stock, List<OrderLine> orderLines, Dictionary<string, Product> products)
        {
            List<string> errors = new List<string>();
            for (int i = 0; i < lineItems.Count; i++)
            {
                var lineItem = lineItems[i];
                var orderLine = orderLines[i];

                if (!stock.ContainsKey(lineItem.ProductId))
                {
                    errors.Add(string.Format("Product: {0}, no stock held", orderLine.gtin));
                    continue;
                }

                var item = stock[lineItem.ProductId];
                if (lineItem.Quantity > item.held)
                {
                    errors.Add(
                        string.Format("Product: {0}, stock held: {1}, stock to remove: {2}", orderLine.gtin, item.held,
                            lineItem.Quantity));
                }

                foreach (var order in orderLines)
                {
                    if (!products.ContainsKey(orderLine.gtin))
                    {
                        errors.Add(string.Format("Unknown product gtin: {0}", orderLine.gtin));
                    }
                }
            }
            return errors;

        }
        public List<Truck> Truck(OutboundOrderRequestModel request)
        {
            //create our list of trucks for the order
            List<Truck> trucks = new List<Truck>();

            var gtins = AddGtins(request);
            Dictionary<string, Product> products = CreateProducts(gtins);
            var lastGtin = "0";

            foreach (var orderLine in request.OrderLines)
            {
                trucks.Add(new Truck()
                {
                    WeightInGrams = 0,
                    Products = new List<Product>()
                });

                for (int i = 0; i < orderLine.quantity; i++)
                {
                    var product = products[orderLine.gtin];

                    //Get the last truck in the list as it should have space left
                    var truck = trucks.Last();
                    // Check that this truck has weight left if it does not, create and use a new truck
                    if (truck.WeightInGrams + product.Weight > truck.MaxWeightInGrams)
                    {
                        trucks.Add(new Truck()
                        {
                            WeightInGrams = 0,
                            Products = new List<Product>()
                        });
                        truck = trucks.Last();
                    }
                    // when we find the right product in the dict, add that product to the truck
                    truck.Products.Add(product);
                    truck.WeightInGrams = truck.WeightInGrams + product.Weight;
                }          
            }
      
            return trucks;
        }
    }
}