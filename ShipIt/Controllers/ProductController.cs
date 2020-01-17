using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ShipIt.Exceptions;
using ShipIt.Models.ApiModels;
using ShipIt.Models.DataModels;
using ShipIt.Parsers;
using ShipIt.Repositories;
using ShipIt.Validators;

namespace ShipIt.Controllers
{
    public class ProductController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IProductRepository productRepository;

        public ProductController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public ProductResponse Get(string gtin)
        {
            if (gtin == null)
            {
                throw new MalformedRequestException("Unable to parse gtin from request parameters");
            }

            log.Info("Looking up product by gtin: " + gtin);
            var product = new Product(productRepository.GetProductByGtin(gtin));
            log.Info("Found product: " + product);

            return new ProductResponse(product);
        }

        public Response Post([FromBody]ProductsRequestModel requestModel)
        {
            var parsedProducts = new List<Product>();

            foreach (var requestProduct in requestModel.Products)
            {
                var parsedProduct = requestProduct.Parse();
                new ProductValidator().Validate(parsedProduct);
                parsedProducts.Add(parsedProduct);
            }

            log.Info("Adding products: " + parsedProducts);
            var dataProducts = parsedProducts.Select(p => new ProductDataModel(p));
            productRepository.AddProducts(dataProducts);           
            log.Debug("Products added successfully");

            return new Response() { Success = true };
        }

        [HttpPatch]
        public Response Discontinue(string gtin)
        {
            if (gtin == null)
            {
                throw new MalformedRequestException("Unable to parse gtin from request parameters");
            }

            log.Info("Discontinuing up product by gtin: " + gtin);
            productRepository.DiscontinueProductByGtin(gtin);
            log.Info("Discontinued product: " + gtin);

            return new Response() { Success = true };
        }
    }
}
