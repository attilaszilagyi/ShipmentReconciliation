using System.Collections.Generic;
using System.Linq;

namespace ShipmentReconciliation
{
  /// <summary>
  /// Reconciliation decisions for one product
  /// </summary>
  public class ResultData
  {
    /// <summary>
    /// Creates result record for one product.
    /// </summary>
    /// <param name="product">ItemName</param>
    /// <param name="quantityShipped">Total quantity of Factory Shipment of the product</param>
    /// <param name="decisions">Which Customer Orders should be fulfilled and which not</param>
    public ResultData(string product, int quantityShipped, IEnumerable<ResultDecision> decisions)
    {
      Product = product;
      Decisions = decisions;
      QuantityShipped = quantityShipped;      
      QuantityFulfill = CustomerOrdersToFulfill.Sum(r => r.Quantity);
      Surplus = quantityShipped - QuantityFulfill;
    }
    /// <summary>
    /// Product name (ItemName)
    /// </summary>
    public string Product { get; private set; }
    /// <summary>
    /// Quantity to store
    /// </summary>
    public int Surplus { get; private set; }
    /// <summary>
    /// Which Customer Orders should be fulfilled and which not.
    /// </summary>
    public IEnumerable<ResultDecision> Decisions { get; private set; }
    /// <summary>
    /// Total quantity of Factory Shipment of the product.
    /// </summary>
    public int QuantityShipped { get; private set; }
    /// <summary>
    /// Total product quantity of Customer Orders to be fulfilled.
    /// </summary>
    public int QuantityFulfill { get; private set; }

    /// <summary>
    /// Customer Orders of the product to fulfill
    /// </summary>
    public IEnumerable<CustomerOrder> CustomerOrdersToFulfill => from record in Decisions
                                                                 where record.Fulfill == true
                                                                 orderby record.CustomerOrder.OrderID
                                                                 select record.CustomerOrder;

  }
}
