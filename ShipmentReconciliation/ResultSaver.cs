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
      CsvFile.WriteToFile(customerOrdersToFulfill, filePath, null, null, null);
    }

    public static void Save(IEnumerable<Tuple<string, int>> productsToStore, string filePath)
    {
      CsvFile.WriteToFile(Enumerate(productsToStore), filePath, null, null, null); 
    }

    public static IEnumerable<FactoryShipment> Enumerate(IEnumerable<Tuple<string, int>> productsToStore)
    {
      foreach (var item in productsToStore)
      {
        yield return new FactoryShipment() { ItemName = item.Item1, Quantity = item.Item2 };
      }
    }
  }
}
