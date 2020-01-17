using System;
using System.Collections.Generic;
using System.Web.Http;
using ShipIt.Exceptions;
using ShipIt.Models.ApiModels;
using ShipIt.Repositories;

namespace ShipIt.Controllers
{
    public class CompanyController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ICompanyRepository companyRepository;

        public CompanyController(ICompanyRepository companyRepository)
        {
            this.companyRepository = companyRepository;
        }

        public CompanyResponse Get(string gcp)
        {
            if (gcp == null)
            {
                throw new MalformedRequestException("Unable to parse gcp from request parameters");
            }

            log.Info(String.Format("Looking up company by name: {0}", gcp));

            var companyDataModel = companyRepository.GetCompany(gcp);
            var company = new Company(companyDataModel);

            log.Info("Found company: " + company.ToString());

            return new CompanyResponse(company);
        }

        public Response Post([FromBody]AddCompaniesRequest requestModel)
        {
            List<Company> companiesToAdd = requestModel.companies;

            if (companiesToAdd.Count == 0)
            {
                throw new MalformedRequestException("Expected at least one <company> tag");
            }

            log.Info("Adding companies: " + companiesToAdd);
            companyRepository.AddCompanies(companiesToAdd);            
            log.Debug("Companies added successfully");

            return new Response() {Success = true};
        }
    }
}
