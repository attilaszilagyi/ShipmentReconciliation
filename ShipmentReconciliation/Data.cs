using System.Collections.Generic;
using System.Linq;

namespace ShipmentReconciliation
{
  internal class Data
  {
    public IList<CustomerOrder> CustomerOrders { get; set; } = new List<CustomerOrder>();

    public IList<FactoryShipment> FactoryShipments { get; set; } = new List<FactoryShipment>();

    public Data()
    {
    }

  }
}