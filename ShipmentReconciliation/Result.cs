using System;
using System.Collections.Generic;
using System.Linq;

namespace ShipmentReconciliation
{
  /// <summary>
  /// Return object holding data of successful reconciliation process of
  /// Customer Orders and Factory Shipments. Has two main properties: 
  /// CustomerOrdersToFulfill and ProductsToStore.
  /// </summary>
  /// <remarks></remarks>
  public class Result
  {

    public Result(IEnumerable<ResultData> data)
    {
      Data = data;
    }

    /// <summary>
    /// Reconciliation decisions per products.
    /// </summary>
    public IEnumerable<ResultData> Data { get; private set; }

    /// <summary>
    /// Returns all customer orders to be fulfilled.
    /// </summary>
    public IEnumerable<CustomerOrder> CustomerOrdersToFulfill
    {
      get
      {
        foreach (ResultData resultData in Data)
        {
          foreach (CustomerOrder item in SelectCustomerOrdersToFulfill(resultData))
          {
            yield return item;
          }
        }
      }
    }

    /// <summary>
    /// Returns the products (item name) and the surplus quantity to store.
    /// </summary>
    public IEnumerable<Tuple<string, int>> ProductsToStore
    {
      get
      {
        foreach (ResultData resultData in Data.Where(r => r.Surplus > 0))
        {
          yield return new Tuple<string, int>(resultData.Product, resultData.Surplus);
        }
      }
    }
    

    private static IEnumerable<CustomerOrder> SelectCustomerOrdersToFulfill(ResultData resultData)
    {
      return
        from record in resultData.Decisions
        where record.Fulfill == true
        orderby record.CustomerOrder.OrderID
        select record.CustomerOrder;
    }

  }
}
