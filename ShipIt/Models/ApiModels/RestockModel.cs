namespace ShipIt.Models.ApiModels
{
    public class RestockModel
    {
        public class RestockingModel
        {
            public int ProductID { get; set; }
            public int WarehouseID { get; set; }
            public int Held { get; set; }
            public string ProductNumber { get; set; }
            public string CompanyID { get; set; }
            public string ProductName { get; set; }
            public double Weight { get; set; }
            public int LowerThreshold { get; set; }
            public int Discontiued { get; set; }
            public int MinimumOrderQuantity { get; set; }
            public string CompanyName { get; set; }
            public string Addr2 { get; set; }
            public string Addr3 { get; set; }
            public string Addr4 { get; set; }
            public string Postcode { get; set; }
            public string City { get; set; }
            public string Phone { get; set; }
            public string Mail { get; set; }
        }
    }
}