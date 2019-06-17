using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ShipmentReconciliation
{
  public static class Reconciler
  {

    public static Result Resolve(DataWrapper dataWrapper, int optimizerLimit, System.Action<string> progressChanged = null, [CallerMemberName] string operation = "")
    {
      Result result = new Result(new HashSet<ResultData>(GetResults(dataWrapper, optimizerLimit, progressChanged, operation)));
      return result;
    }

    public static IEnumerable<ResultData> GetResults(DataWrapper dataWrapper, int optimizerLimit, System.Action<string> progressChanged, string operation)
    {


      foreach (KeyValuePair<string, int> item in dataWrapper.Balance)
      {
        string product = item.Key;
        int balance = item.Value;
        int shipped = dataWrapper.SumFactoryShipments[product];

        if (balance == 0)
        {
          //All orders fulfill, nothing to store
          yield return new ResultData(product, shipped, ResultDecision.Create(dataWrapper.GetCustomerOrdersByItemName(product), true));
        }

        if (balance > 0)
        {
          //All orders fulfill, there are some product item from the factory shipments to store
          yield return new ResultData(product, shipped, ResultDecision.Create(dataWrapper.GetCustomerOrdersByItemName(product), true));
        }

        if (balance < 0)
        {
          //Some orders may be fulfilled, but not all. Optimization needed. Some product items from the factory shipments might be to store.

          IEnumerable<ResultDecision> decisionsSimple = ResolverSimple.Resolve(shipped, dataWrapper.GetCustomerOrdersByItemName(product), out double efficiencySimple);
          if (efficiencySimple < 1)
          {
            IEnumerable<ResultDecision> decisionsComplex = ResolverComplex.Resolve(optimizerLimit, shipped, dataWrapper.GetCustomerOrdersByItemName(product).ToArray(), efficiencySimple, out double efficiencyComplex);
            if (efficiencyComplex > efficiencySimple)
            {
              yield return new ResultData(product, shipped, decisionsComplex);
            }
          }

          yield return new ResultData(product, shipped, decisionsSimple);
        }

      }


    }

  }
}
