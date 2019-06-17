using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ShipmentReconciliation
{
  public static class Reconciler
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataWrapper"></param>
    /// <param name="optimizerLimit">Maximum number of different combinations to try. Zero: no limit.</param>
    /// <param name="progressChanged"></param>
    /// <param name="operation"></param>
    /// <returns></returns>
    public static Result Resolve(DataWrapper dataWrapper, int optimizerLimit, System.Action<string> progressChanged = null, [CallerMemberName] string operation = "")
    {
      progressChanged?.Invoke($"{operation}");
      Result result = new Result(new HashSet<ResultData>(GetResults(dataWrapper, optimizerLimit, progressChanged, operation)));
      progressChanged?.Invoke($"{operation} {result.}");
      return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataWrapper"></param>
    /// <param name="optimizerLimit">Maximum number of different combinations to try. Zero: no limit.</param>
    /// <param name="progressChanged"></param>
    /// <param name="operation"></param>
    /// <returns></returns>
    public static IEnumerable<ResultData> GetResults(DataWrapper dataWrapper, int optimizerLimit, System.Action<string> progressChanged, string operation)
    {
      int cntMax = dataWrapper.Balance.Count;
      int cnt = 0;
      foreach (KeyValuePair<string, int> item in dataWrapper.Balance)
      {        
        string product = item.Key;
        progressChanged?.Invoke($"{operation} {++cnt}/{cntMax} {product}");
        int balance = item.Value;
        int shipped = dataWrapper.SumFactoryShipments.ContainsKey(product) ? dataWrapper.SumFactoryShipments[product] : 0;
        if (shipped == 0)
        {
          //None of the orders can be fulfilled, but there is nothing to store
          yield return new ResultData(product, shipped, ResultDecision.Create(dataWrapper.GetCustomerOrdersByItemName(product), fulfill: false));
        }
        else
        if (balance == 0)
        {
          //All orders fulfill, nothing to store
          yield return new ResultData(product, shipped, ResultDecision.Create(dataWrapper.GetCustomerOrdersByItemName(product), fulfill: true));
        }
        else
        if (balance > 0)
        {
          //All orders fulfill, there are some product item from the factory shipments to store
          yield return new ResultData(product, shipped, ResultDecision.Create(dataWrapper.GetCustomerOrdersByItemName(product), fulfill: true));
        }
        else
        if (balance < 0)
        {
          //Some orders may be fulfilled, but not all. Optimization needed. Some product items from the factory shipments might be to store.
          IEnumerable<ResultDecision> decisionsComplex = null;double efficiencyComplex = 0;
          IEnumerable<ResultDecision> decisionsSimple = ResolverSimple.Resolve(shipped, dataWrapper.GetCustomerOrdersByItemName(product), out double efficiencySimple);
          if (efficiencySimple < 1)
          {
            decisionsComplex = ResolverComplex.Resolve(optimizerLimit, shipped, dataWrapper.GetCustomerOrdersByItemName(product).ToArray(), efficiencySimple, out efficiencyComplex);            
          }
          if (efficiencyComplex > efficiencySimple)
          {
            yield return new ResultData(product, shipped, decisionsComplex);
          }
          else
          {
            yield return new ResultData(product, shipped, decisionsSimple);
          }
        }

      }


    }

  }
}
