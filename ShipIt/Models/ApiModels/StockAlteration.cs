using ShipIt.Exceptions;

namespace ShipIt.Models.ApiModels
{
    public class StockAlteration
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public StockAlteration(int productId, int quantity)
        {
            this.ProductId = productId;
            this.Quantity = quantity;

            if (quantity < 0)
            {
                throw new MalformedRequestException("Alteration must be positive");
            }
        }
    }
}