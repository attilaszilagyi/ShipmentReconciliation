using System.Collections.Generic;
using System.Linq;

namespace ShipmentReconciliation
{
  /// <summary>
  /// Records of Customer Order and Factory Shipment data.
  /// </summary>
  /// <remarks>Populated by generating test data or loading csv contents from file system.</remarks>
  internal class Data
  {
    /// <summary>
    /// Customer order records.
    /// </summary>
    public IList<CustomerOrder> CustomerOrders { get; set; } = new List<CustomerOrder>();

    /// <summary>
    /// Factory Shipment records
    /// </summary>
    public IList<FactoryShipment> FactoryShipments { get; set; } = new List<FactoryShipment>();

  }
}