using System.Collections.Generic;

namespace ShipmentReconciliation
{
  /// <summary>
  /// Simple solver of 01-Knapsack problem
  /// </summary>
  public static class ResolverSimple
  {
    /// <summary>
    /// Simple algorithm to solve of 01-Knapsack problem
    /// </summary>
    /// <param name="shipped"></param>
    /// <param name="orders"></param>
    /// <param name="efficiency"></param>
    /// <returns></returns>
    /// <remarks>Works on sorted "weights", not recursive, only checks to swap the last added element in the "bag".</remarks>
    public static IEnumerable<ResultDecision> Resolve(int shipped, IEnumerable<CustomerOrder> orders, out double efficiency)
    {
      HashSet<ResultDecision> results = new HashSet<ResultDecision>();
      int total = 0;
      ResultDecision last = null;
      int lastTotal = 0;
      foreach (CustomerOrder item in orders)
      {
        if (total + item.Quantity <= shipped)
        {
          ResultDecision rd = new ResultDecision(item, true);
          results.Add(rd);
          lastTotal = total;
          total += item.Quantity;
          last = rd;
        }
        else if (last != null && lastTotal + item.Quantity <= shipped && lastTotal + item.Quantity > total)
        {
          results.Remove(last);
          results.Add(new ResultDecision(last.CustomerOrder, false));
          
          ResultDecision rd = new ResultDecision(item, true);
          results.Add(rd);
          total = lastTotal + item.Quantity;
          lastTotal = total;
          last = rd;
        }
        else
        { results.Add(new ResultDecision(item, false)); }
      }
      efficiency = total / (double)shipped;
      return results;
    }

  }
}
