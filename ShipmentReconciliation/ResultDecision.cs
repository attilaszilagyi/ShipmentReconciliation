using System.Collections.Generic;

namespace ShipmentReconciliation
{
  public class ResultDecision
  {
    public CustomerOrder CustomerOrder { get; private set; }
    public bool Fulfill { get; private set; }
    public ResultDecision(CustomerOrder customerOrder, bool fulfill)
    {
      CustomerOrder = customerOrder;
      Fulfill = fulfill;
    }


    public static IEnumerable<ResultDecision> Create(IEnumerable<CustomerOrder> customerOrders, bool fulfill)
    {
      foreach (var item in customerOrders)
      {
        yield return new ResultDecision(item, fulfill);
      }
    }


  }
}
