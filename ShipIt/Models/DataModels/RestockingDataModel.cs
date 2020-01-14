using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ShipIt.Models.DataModels
{
    public class RestockingDataModel: DataModel
    {
        [DatabaseColumnName("p_id")]
        public int ProductID { get; set; }
        [DatabaseColumnName("w_id")]
        public int WarehouseID { get; set; }
        [DatabaseColumnName("hld")]
        public int Held { get; set; }
        [DatabaseColumnName("gtin_cd")]
        public string ProductNumber { get; set; }
        [DatabaseColumnName("gcp_cd")]
        public string CompanyID { get; set; }
        [DatabaseColumnName("gtin_nm")]
        public string ProductName { get; set; }
        [DatabaseColumnName("m_g")]
        public double Weight { get; set; }
        [DatabaseColumnName("l_th")]
        public int LowerThreshold { get; set; }
        [DatabaseColumnName("ds")]
        public int Discontiued { get; set; }
        [DatabaseColumnName("min_qt")]
        public int MinimumOrderQuantity { get; set; }
        [DatabaseColumnName("gln_nm")]
        public string CompanyName { get; set; }
        [DatabaseColumnName("gln_addr_02")]
        public string Addr2 { get; set; }
        [DatabaseColumnName("gln_addr_03")]
        public string Addr3 { get; set; }
        [DatabaseColumnName("gln_addr_04")]
        public string Addr4 { get; set; }
        [DatabaseColumnName("gln_addr_postalcode")]
        public string Postcode { get; set; }
        [DatabaseColumnName("gln_addr_city")]
        public string City { get; set; }
        [DatabaseColumnName("contact_tel")]
        public string Phone { get; set; }
        [DatabaseColumnName("contact_mail")]
        public string Mail { get; set; }

        public RestockingDataModel(IDataReader dataReader) : base(dataReader)
        {
        }

    }
}