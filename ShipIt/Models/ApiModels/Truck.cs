using System.Collections.Generic;

namespace ShipIt.Models.ApiModels
{
    public class Truck
    {
        public float MaxWeightInGrams = 2000000;
        public float WeightInGrams = 0;
        public List<Product> Products { get; set; }
    }
}