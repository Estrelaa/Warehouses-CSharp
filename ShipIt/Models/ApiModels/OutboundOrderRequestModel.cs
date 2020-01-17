using System;
using System.Collections.Generic;
using System.Text;

namespace ShipIt.Models.ApiModels
{
    public class OutboundOrderRequestModel
    {
        public int WarehouseId { get; set; }
        public IEnumerable<OrderLine> OrderLines { get; set; }

        public override String ToString()
        {
            return new StringBuilder()
                .AppendFormat("warehouseId: {0}, ", WarehouseId)
                .AppendFormat("orderLines: {0}", OrderLines)
                .ToString();
        }
    }
}
