using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentReconciliation
{
  public static class ResolverSimple
  {   

    public static IEnumerable<ResultDecision> Resolve(int shipped, IEnumerable<CustomerOrder> orders, out double efficiency)
    {
      var results = new HashSet<ResultDecision>();
      int total = 0;
      foreach (var item in orders)
      {
        if(total + item.Quantity <= shipped)
        {
          results.Add(new ResultDecision(item, true));
          total += item.Quantity;
        }
        else
        { results.Add(new ResultDecision(item, false)); }
      }
      efficiency = (double)total / (double)shipped;
      return results;
    }

  }
}
