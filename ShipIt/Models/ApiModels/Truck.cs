using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipIt.Models.ApiModels
{
    public class Truck
    {
        public int MaxWeightInGrams = 2000000;
        public int WeightInGrams { get; set; }
        public List<OrderLine> Products { get; set; }
    }
}