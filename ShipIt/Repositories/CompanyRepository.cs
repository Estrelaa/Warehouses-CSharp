﻿using System.Configuration;
using System.Data;
using Npgsql;
using ShipIt.Models.DataModels;

namespace ShipIt.Repositories
{
    public interface ICompanyRepository
    {
        int GetCount();
        CompanyDataModel GetCompany(string gcp);
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
                "SELECT gcp_cd, gln_nm, gln_addr_02, gln_addr_03, gln_addr_04, gln_addr_postalcode, gln_addr_city, contact_tel, contact_mail " +
                "FROM gcp " +
                "WHERE gcp_cd = @gcp_cd";
            var parameter = new NpgsqlParameter("@gcp_cd", gcp);
            string noProductWithIdErrorMessage = $"No companies found with gcp: {gcp}";
            return base.RunSingleGetQuery(sql, reader => new CompanyDataModel(reader), noProductWithIdErrorMessage, parameter);
        }

    }

}