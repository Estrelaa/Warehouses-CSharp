using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShipIt.Controllers;
using ShipIt.Exceptions;
using ShipIt.Models.ApiModels;
using ShipIt.Models.DataModels;
using ShipIt.Repositories;
using ShipItTest.Builders;
using System;
using System.Collections.Generic;

namespace ShipItTest
{
    [TestClass]
    public class ProductControllerTests : AbstractBaseTest
    {
        ProductController productController = new ProductController(new ProductRepository());
        ProductRepository productRepository = new ProductRepository();

        private const int WAREHOUSE_ID = 1;

        // private static readonly Employee EMPLOYEE = new EmployeeBuilder().setWarehouseId(WAREHOUSE_ID).CreateEmployee();
        private const string GTIN = "0000346374230";

        [TestMethod]
        public void TestRoundtripProductRepository()
        {
            onSetUp();
            var product = new ProductBuilder().CreateProductDatabaseModel();
            productRepository.AddProducts(new List<ProductDataModel>() {product});
            Assert.AreEqual(productRepository.GetProductByGtin(product.Gtin).Name, product.Name);
            Assert.AreEqual(productRepository.GetProductByGtin(product.Gtin).Gtin, product.Gtin);
        }

        [TestMethod]
        public void TestGetProduct()
        {
            onSetUp();
            var productBuilder = new ProductBuilder().setGtin(GTIN);
            productRepository.AddProducts(new List<ProductDataModel>() {productBuilder.CreateProductDatabaseModel()});
            var result = productController.Get(GTIN);

            var correctProduct = productBuilder.CreateProduct();
            Assert.IsTrue(ProductsAreEqual(correctProduct, result.Product));
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public void TestGetNonexistentProduct()
        {
            onSetUp();
            try
            {
                productController.Get(GTIN);
            }
            catch (NoSuchEntityException e)
            {
                Assert.IsTrue(e.Message.Contains(GTIN));
            }
        }

        [TestMethod]
        public void TestAddProducts()
        {
            onSetUp();
            var productBuilder = new ProductBuilder().setGtin(GTIN);
            var productRequest = productBuilder.CreateProductRequest();

            var response = productController.Post(productRequest);
            var databaseProduct = productRepository.GetProductByGtin(GTIN);
            var correctDatabaseProduct = productBuilder.CreateProductDatabaseModel();

            Assert.IsTrue(response.Success);
            ProductsAreEqual(new Product(databaseProduct), new Product(correctDatabaseProduct));
        }

        [TestMethod]
        public void TestAddPreexistingProduct()
        {
            onSetUp();
            var productBuilder = new ProductBuilder().setGtin(GTIN);
            productRepository.AddProducts(new List<ProductDataModel>() {productBuilder.CreateProductDatabaseModel()});
            var productRequest = productBuilder.CreateProductRequest();

            try
            {
                productController.Post(productRequest);
                Assert.Fail();
            }
            catch (MalformedRequestException e)
            {
                Assert.IsTrue(e.Message.Contains(GTIN));
            }
        }

        [TestMethod]
        public void TestAddDuplicateProduct()
        {
            onSetUp();
            var productBuilder = new ProductBuilder().setGtin(GTIN);
            var productRequest = productBuilder.CreateDuplicateProductRequest();

            try
            {
                productController.Post(productRequest);
                Assert.Fail("Expected exception to be thrown.");
            }
            catch (MalformedRequestException e)
            {
                Assert.IsTrue(e.Message.Contains(GTIN));
            }
        }

        [TestMethod]
        public void TestDiscontinueProduct()
        {
            onSetUp();
            var productBuilder = new ProductBuilder().setGtin(GTIN);
            productRepository.AddProducts(new List<ProductDataModel>() { productBuilder.CreateProductDatabaseModel() });

            productController.Discontinue(GTIN);
            var result = productController.Get(GTIN);

            Assert.IsTrue(result.Product.Discontinued);
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public void TestDiscontinueNonexistentProduct()
        {
            onSetUp();
            try
            {
                productController.Discontinue(GTIN);
                Assert.Fail("Expected exception to be thrown.");
            }
            catch (NoSuchEntityException e)
            {
                Assert.IsTrue(e.Message.Contains(GTIN));
            }
        }

        [TestMethod]
        public void TestDiscontinueNonexistantProduct()
        {
            onSetUp();
            var nonExistantGtin = "12345678";
            try
            {
                productController.Discontinue(nonExistantGtin);
                Assert.Fail("Expected exception to be thrown.");
            }
            catch (NoSuchEntityException e)
            {
                Assert.IsTrue(e.Message.Contains(nonExistantGtin));
            }
        }

        private bool ProductsAreEqual(Product A, Product B)
        {
            const double floatingPointTolerance = 10 * float.Epsilon;
            return A.Discontinued == B.Discontinued
                   && A.Gcp == B.Gcp
                   && A.Gtin == B.Gtin
                   && A.LowerThreshold == B.LowerThreshold
                   && A.MinimumOrderQuantity == B.MinimumOrderQuantity
                   && A.Name == B.Name
                   && Math.Abs(A.Weight - B.Weight) < floatingPointTolerance;
        }
    }
}
