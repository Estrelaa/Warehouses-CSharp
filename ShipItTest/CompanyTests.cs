using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShipIt.Controllers;
using ShipIt.Exceptions;
using ShipIt.Models.ApiModels;
using ShipIt.Repositories;
using ShipItTest.Builders;
using System.Collections.Generic;

namespace ShipItTest
{
    [TestClass]
    public class CompanyControllerTests : AbstractBaseTest
    {
        CompanyController companyController = new CompanyController(new CompanyRepository());
        CompanyRepository companyRepository = new CompanyRepository();

        private const string GCP = "0000346";

        [TestMethod]
        public void TestRoundtripCompanyRepository()
        {
            onSetUp();
            var company = new CompanyBuilder().CreateCompany();
            companyRepository.AddCompanies(new List<Company>() { company });
            Assert.AreEqual(companyRepository.GetCompany(company.Gcp).Name, company.Name);
        }

        [TestMethod]
        public void TestGetCompanyByGcp()
        {
            onSetUp();
            var companyBuilder = new CompanyBuilder().setGcp(GCP);
            companyRepository.AddCompanies(new List<Company>() { companyBuilder.CreateCompany() });
            var result = companyController.Get(GCP);

            var correctCompany = companyBuilder.CreateCompany();
            Assert.IsTrue(CompaniesAreEqual(correctCompany, result.Company));
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public void TestGetNonExistentCompany()
        {
            onSetUp();
            try
            {
                companyController.Get(GCP);
                Assert.Fail("Expected exception to be thrown.");
            }
            catch (NoSuchEntityException e)
            {
                Assert.IsTrue(e.Message.Contains(GCP));
            }
        }

        [TestMethod]
        public void TestAddCompanies()
        {
            onSetUp();
            var companyBuilder = new CompanyBuilder().setGcp(GCP);
            var addCompaniesRequest = companyBuilder.CreateAddCompaniesRequest();

            var response = companyController.Post(addCompaniesRequest);
            var databaseCompany = companyRepository.GetCompany(GCP);
            var correctCompany = companyBuilder.CreateCompany();

            Assert.IsTrue(response.Success);
            Assert.IsTrue(CompaniesAreEqual(new Company(databaseCompany), correctCompany));
        }

        private bool CompaniesAreEqual(Company A, Company B)
        {
            return A.Gcp == B.Gcp
                   && A.Name == B.Name
                   && A.Addr2 == B.Addr2
                   && A.Addr3 == B.Addr3
                   && A.Addr4 == B.Addr4
                   && A.PostalCode == B.PostalCode
                   && A.City == B.City
                   && A.Tel == B.Tel
                   && A.Mail == B.Mail;
        }
    }
}
