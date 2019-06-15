using System;
using System.Collections.Generic;

namespace ShipmentReconciliation
{
  /// <summary>
  /// Generate test data
  /// </summary>
  internal static class DataGenerator
  {

    public static Data Generate(int maxNumberOfProducts, int maxNumberOfOrders, int maxNumberOfCustomers, int maxQuantityPerOrder, int maxTotalQuantityPerProduct)
    {
      Data data = new Data
      {
        CustomerOrders = GenerateCustomerOrders(maxNumberOfProducts, maxNumberOfOrders, maxNumberOfCustomers, maxQuantityPerOrder, maxTotalQuantityPerProduct),
        FactoryShipments = GenerateFactoryShipments(maxNumberOfProducts, maxTotalQuantityPerProduct)
      };
      return data;
    }

    private static IList<FactoryShipment> GenerateFactoryShipments(int maxNumberOfProducts, int maxTotalQuantityPerProduct)
    {
      List<FactoryShipment> records = new List<FactoryShipment>();
      Random random = new Random(DateTime.Now.Millisecond);
      int numberOfProducts = (int)(random.NextDouble() * maxNumberOfProducts);
      for (int iProduct = 1; iProduct <= numberOfProducts; iProduct++)
      {
        string product = "Product" + iProduct;
        int maxQuantity = (int)(random.NextDouble() * maxTotalQuantityPerProduct);
        var total = 0;
        while (total< maxQuantity)
        {
          int quantity = (int)(random.NextDouble() * maxQuantity);
          records.Add(new FactoryShipment()
          {
            ItemName = product,
            Quantity = quantity
          });
          total += quantity;
        }
      }
      return records;
    }

    private static IList<CustomerOrder> GenerateCustomerOrders(int maxNumberOfProducts, int maxNumberOfOrders, int maxNumberOfCustomers, int maxQuantityPerOrder, int maxTotalQuantityPerProduct)
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
      }
      return records;
    }
  }
}
