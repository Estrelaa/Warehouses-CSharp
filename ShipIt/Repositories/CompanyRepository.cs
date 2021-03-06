﻿using Npgsql;
using ShipIt.Models.ApiModels;
using ShipIt.Models.DataModels;
using System.Collections.Generic;
using System.Linq;

namespace ShipIt.Repositories
{
    public interface ICompanyRepository
    {
        int GetCount();
        CompanyDataModel GetCompany(string gcp);
        void AddCompanies(IEnumerable<Company> companies);
        List<CompanyDataModel> GetAllCompanies();
    }

    public class CompanyRepository : RepositoryBase, ICompanyRepository
    {
        public int GetCount()
        {
            string CompanyCountSQL = "SELECT COUNT(*) FROM gcp";
            return (int)QueryForLong(CompanyCountSQL);
        }

        public CompanyDataModel GetCompany(string gcp)
        {
            string sql =
                "SELECT *" +
                "FROM gcp " +
                "WHERE gcp_cd = @gcp_cd";
            var parameter = new NpgsqlParameter("@gcp_cd", gcp);
            string noProductWithIdErrorMessage = string.Format("No companies found with gcp: {0}", gcp);
            return RunSingleGetQuery(sql, reader => new CompanyDataModel(reader), noProductWithIdErrorMessage, parameter);
        }
        public List<CompanyDataModel> GetAllCompanies()
        {
            string sql =
                "SELECT *" +
                "FROM gcp ";
            string noProductWithIdErrorMessage = string.Format("No companies found");
            return RunGetQuery(sql, reader => new CompanyDataModel(reader), noProductWithIdErrorMessage).ToList();
        }

        public void AddCompanies(IEnumerable<Company> companies)
        {
            string sql =
                "INSERT INTO gcp (gcp_cd, gln_nm, gln_addr_02, gln_addr_03, gln_addr_04, gln_addr_postalcode, gln_addr_city, contact_tel, contact_mail) " +
                "VALUES (@gcp_cd, @gln_nm, @gln_addr_02, @gln_addr_03, @gln_addr_04, @gln_addr_postalcode, @gln_addr_city, @contact_tel, @contact_mail)";

            var parametersList = new List<NpgsqlParameter[]>();
            foreach (var company in companies)
            {
                var companyDataModel = new CompanyDataModel(company);
                parametersList.Add(companyDataModel.GetNpgsqlParameters().ToArray());
            }
            RunTransaction(sql, parametersList);
        }
    }
}