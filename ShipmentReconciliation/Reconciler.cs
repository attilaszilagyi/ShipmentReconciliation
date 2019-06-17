using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentReconciliation
{
  public static class Reconciler
  {

    public static Result Resolve(DataWrapper dataWrapper, System.Action<string> progressChanged = null, [CallerMemberName] string operation = "")
    {
      Result result = new Result(new HashSet<ResultData>( GetResults(dataWrapper, progressChanged, operation)));
      return result;
    }

    public static IEnumerable< ResultData> GetResults(DataWrapper dataWrapper, System.Action<string> progressChanged, string operation)
    {
      

      foreach (var item in dataWrapper.Balance)
      {
        string product = item.Key;
        int balance = item.Value;

        if (balance == 0)
        {
          //All orders fulfill, nothing to store
          yield return new ResultData(product, dataWrapper.SumFactoryShipments[product], ResultDecision.Create(dataWrapper.GetCustomerOrdersByItemName(product), true));
        }

        if (balance > 0)
        {
          //All orders fulfill, there are some product item from the factory shipments to store
          yield return new ResultData(product, dataWrapper.SumFactoryShipments[product], ResultDecision.Create(dataWrapper.GetCustomerOrdersByItemName(product), true));
        }
        if (balance < 0)
        {
          //Some orders may be fulfilled, but not all. Optimization needed. Some product items from the factory shipments might be to store.


          continue;
        }

      }

      
    }

  }
}
