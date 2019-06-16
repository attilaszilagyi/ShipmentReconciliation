using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ShipmentReconciliation
{
  /// <summary>
  /// Generate test data
  /// </summary>
  internal static partial class DataGenerator
  {
    /// <summary>
    /// Generates test data records with random item names, customer ids and quantities.
    /// </summary>
    /// <param name="maxNumberOfProducts"></param>
    /// <param name="maxNumberOfOrders"></param>
    /// <param name="maxNumberOfCustomers"></param>
    /// <param name="maxQuantityPerOrder"></param>
    /// <param name="maxTotalQuantityPerProduct"></param>
    /// <param name="progressChanged">Callback for progress report</param>
    /// <param name="operation">Title text for progress report</param>
    /// <returns></returns>
    public static Data Generate(int maxNumberOfProducts, int maxNumberOfOrders, int maxNumberOfCustomers, int maxQuantityPerOrder, int maxTotalQuantityPerProduct, System.Action<string> progressChanged = null, [CallerMemberName] string operation = "")
    {
      Status status = new Status(operation, progressChanged, 100);
      IList<CustomerOrder> customerOrders = null;
      IList<FactoryShipment> factoryShipments = null;
      Task.WaitAll(                
                Task.Factory.StartNew(() => customerOrders = GenerateCustomerOrders(maxNumberOfProducts, maxNumberOfOrders, maxNumberOfCustomers, maxQuantityPerOrder, maxTotalQuantityPerProduct, status, progressChanged, operation)),
                Task.Factory.StartNew(() => factoryShipments = GenerateFactoryShipments(maxNumberOfProducts, maxTotalQuantityPerProduct, status, progressChanged, operation))
                );
      status.Report();
      Data data = new Data
      {
        CustomerOrders = customerOrders,
        FactoryShipments = factoryShipments
      };
      return data;
    }

    private static IList<FactoryShipment> GenerateFactoryShipments(int maxNumberOfProducts, int maxTotalQuantityPerProduct, Status status, System.Action<string> progressChanged, string operation)
    {
      List<FactoryShipment> records = new List<FactoryShipment>();
      Random random = new Random(DateTime.Now.Millisecond);
      int numberOfProducts = (int)(random.NextDouble() * maxNumberOfProducts);
      for (int iProduct = 1; iProduct <= numberOfProducts; iProduct++)
      {
        string product = "Product" + iProduct;
        int maxQuantity = (int)(random.NextDouble() * maxTotalQuantityPerProduct);
        var total = 0;
        if (maxQuantity > 0)
        {
          while (total < maxQuantity)
          {
            int quantity = Math.Max((int)(random.NextDouble() * maxQuantity), 1);
            records.Add(new FactoryShipment()
            {
              ItemName = product,
              Quantity = quantity
            });
            total += quantity;
            status.FactoryShipmentCount++;
            
          }
        }
      }
      return records;
    }

    private static IList<CustomerOrder> GenerateCustomerOrders(int maxNumberOfProducts, int maxNumberOfOrders, int maxNumberOfCustomers, int maxQuantityPerOrder, int maxTotalQuantityPerProduct, Status status, System.Action<string> progressChanged, string operation)
    {
      List<CustomerOrder> records = new List<CustomerOrder>();
      Random random = new Random(DateTime.Now.Millisecond);
      int numberOfProducts = (int)(random.NextDouble() * maxNumberOfProducts);
      int total = 0;
      for (int iOrder = 1, iProduct = 1; iOrder <= maxNumberOfOrders && iProduct <= numberOfProducts; iOrder++)
      {
        int quantity = (int)(random.NextDouble() * maxQuantityPerOrder);
        total += quantity;
        if (total > maxTotalQuantityPerProduct)
        {
          total = 0;
          iProduct++;
          continue;
        }
        records.Add(new CustomerOrder()
        {
          OrderID = iOrder,
          CustomerID = random.Next() % maxNumberOfCustomers + 1,
          ItemName = "Product" + iProduct,
          Quantity = quantity
        });
        status.CustomerOrderCount++;
        
      }
      return records;
    }
  }
}
