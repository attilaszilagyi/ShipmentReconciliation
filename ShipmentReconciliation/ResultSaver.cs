using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentReconciliation
{
 public static class ResultSaver
  {
    public static void Save(IEnumerable<CustomerOrder> customerOrdersToFulfill, string filePath)
    {
      throw new NotImplementedException();
    }

    public static void Save(IEnumerable<Tuple<string, int>> productsToStore, string filePath)
    { throw new NotImplementedException(); }
  }
}
