using System.Collections.Generic;
using System.Linq;

namespace ShipmentReconciliation
{
  public class ResultData
  {

    public ResultData(string product, int quantityShipped, IEnumerable<ResultDecision> decisions)
    {
      Product = product;
      Decisions = decisions;
      QuantityShipped = quantityShipped;      
      QuantityFulfill = CustomerOrdersToFulfill.Sum(r => r.Quantity);
      Surplus = quantityShipped - QuantityFulfill;
    }

    public string Product { get; private set; }
    public int Surplus { get; private set; }
    public IEnumerable<ResultDecision> Decisions { get; private set; }
    //public int QuantityOrdered { get; private set; }
    public int QuantityShipped { get; private set; }
    public int QuantityFulfill { get; private set; }


    public IEnumerable<CustomerOrder> CustomerOrdersToFulfill => from record in Decisions
                                                                 where record.Fulfill == true
                                                                 orderby record.CustomerOrder.OrderID
                                                                 select record.CustomerOrder;

  }
}
