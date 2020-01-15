using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipIt.Models.ApiModels
{
    public class Truck
    {
        public int weight = 0; // in grams
        public List<Product> products = new List<Product>();
    }
}