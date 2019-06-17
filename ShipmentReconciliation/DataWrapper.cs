using System.Collections.Generic;
using System.Linq;

namespace ShipmentReconciliation
{
  /// <summary>
  /// Provides some pre-processed input data: 
  /// Customer Orders grouped by Item Name, 
  /// summed quantities grouped by Item Name, 
  /// record counts and quantity balance,
  /// total surplus and deficit
  /// </summary>
  /// <remarks>Also provides some static helper methods to order and group data records.</remarks>
  public class DataWrapper
  {
    /// <summary>
    /// Original records
    /// </summary>
    public Data Data { get; }
    /// <summary>
    /// Summed Quantities of Customer Orders grouped by Item Name
    /// </summary>
    public Dictionary<string, int> SumCustomerOrders { get; private set; }
    /// <summary>
    /// Summed Quantities of Factory Shipments grouped by Item Name
    /// </summary>
    public Dictionary<string, int> SumFactoryShipments { get; private set; }
    /// <summary>
    /// ShippedQuantity - OrderedQuantity per products. Positive value: we have more than enough items to fullfill the orders. Negative value: we have less items than needed to fullfill the orders.
    /// </summary>
    public Dictionary<string, int> Balance { get; private set; }
    /// <summary>
    /// Sum of all excess in all products
    /// </summary>
    public int TotalSurplus { get; private set; }
    /// <summary>
    /// Sum of all shortage in all products
    /// </summary>
    public int TotalDeficit { get; private set; }

    public int CountItemCustomerOrders { get; private set; }
    public int CountItemFactoryShipment { get; private set; }
    public int CountRecordCustomerOrders { get; private set; }
    public int CountRecordFactoryShipment { get; private set; }
    public int CountProductCustomerOrders { get; private set; }
    public int CountProductFactoryShipment { get; private set; }
    public int CountProductDeficit { get; private set; }
    public int CountProductSurplus { get; private set; }

    public DataWrapper(Data data)
    {
      Data = data;
      Recalculate();
    }

    /// <summary>
    /// Pre-processes Data records. Calculates sums, counts, and aggregated balances of quantities by products.
    /// </summary>
    /// <remarks>You may call this method explicitly only after the associated Data record collections changed.</remarks>
    public void Recalculate()
    {
      SumFactoryShipments = CalculateSum(GetFactoryShipmentsGroupedByItemName());
      SumCustomerOrders = CalculateSum(GetCustomerOrdersGroupedByItemName());
      Balance = new Dictionary<string, int>();
      foreach (KeyValuePair<string, int> item in SumCustomerOrders)
      {
        CountItemCustomerOrders += item.Value;
        int diff = (SumFactoryShipments.ContainsKey(item.Key) ? SumFactoryShipments[item.Key] : 0) - item.Value;
        Balance[item.Key] = diff;
        if (diff > 0)
        {
          TotalSurplus += diff;
          CountProductSurplus++;
        }
        else
        {
          TotalDeficit += diff;
          CountProductDeficit++;
        }
      }
      foreach (KeyValuePair<string, int> item in SumFactoryShipments)
      {
        CountItemFactoryShipment += item.Value;
        if (!Balance.ContainsKey(item.Key))
        { Balance[item.Key] = -item.Value; TotalSurplus += item.Value; CountProductSurplus++; }
      }
      CountRecordFactoryShipment = Data.FactoryShipments.Count;
      CountRecordCustomerOrders = Data.CustomerOrders.Count;
      CountProductCustomerOrders = SumCustomerOrders.Count;
      CountProductFactoryShipment = SumFactoryShipments.Count;
    }

    /// <summary>
    /// Returns total quantites per products.
    /// </summary>
    /// <param name="groupedRecords"></param>
    /// <returns></returns>
    public static Dictionary<string, int> CalculateSum(IOrderedEnumerable<IGrouping<string, CustomerOrder>> groupedRecords)
    {
      Dictionary<string, int> total = new Dictionary<string, int>();
      foreach (IGrouping<string, CustomerOrder> group in groupedRecords)
      {
        total[group.Key] = 0;
        foreach (CustomerOrder item in group)
        {
          total[group.Key] += item.Quantity;
        }
      }
      return total;
    }

    /// <summary>
    /// Returns total quantites per products.
    /// </summary>
    /// <param name="groupedRecords"></param>
    /// <returns></returns>
    public static Dictionary<string, int> CalculateSum(IOrderedEnumerable<IGrouping<string, FactoryShipment>> groupedRecords)
    {
      Dictionary<string, int> total = new Dictionary<string, int>();
      foreach (IGrouping<string, FactoryShipment> group in groupedRecords)
      {
        total[group.Key] = 0;
        foreach (FactoryShipment item in group)
        {
          total[group.Key] += item.Quantity;
        }
      }
      return total;
    }

    /// <summary>
    /// Groups records by products.
    /// </summary>
    /// <returns></returns>
    public IOrderedEnumerable<IGrouping<string, CustomerOrder>> GetCustomerOrdersGroupedByItemName()
    {
      return GetCustomerOrdersGroupedByItemName(Data);
    }

    /// <summary>
    /// Groups records by products.
    /// </summary>
    /// <returns></returns>
    public static IOrderedEnumerable<IGrouping<string, CustomerOrder>> GetCustomerOrdersGroupedByItemName(Data data)
    {
      return
        from record in data.CustomerOrders
        group record by record.ItemName into itemNameGroup
        orderby itemNameGroup.Key
        select itemNameGroup;
    }

    /// <summary>
    /// Groups records by products.
    /// </summary>
    /// <returns></returns>
    public IOrderedEnumerable<IGrouping<string, FactoryShipment>> GetFactoryShipmentsGroupedByItemName()
    {
      return GetFactoryShipmentsGroupedByItemName(Data);
    }

    /// <summary>
    /// Groups records by products.
    /// </summary>
    /// <returns></returns>
    public static IOrderedEnumerable<IGrouping<string, FactoryShipment>> GetFactoryShipmentsGroupedByItemName(Data data)
    {
      return
       from record in data.FactoryShipments
       group record by record.ItemName into itemNameGroup
       orderby itemNameGroup.Key
       select itemNameGroup;
    }

    /// <summary>
    /// Returns records filtered to a product, and sorted ascending by quantity.
    /// </summary>
    /// <returns></returns>
    public IOrderedEnumerable< CustomerOrder> GetCustomerOrdersByItemName(string itemName)
    {
      return GetCustomerOrdersByItemName(Data, itemName);
    }

    /// <summary>
    /// Returns records filtered to a product, and sorted ascending by quantity.
    /// </summary>
    /// <returns></returns>
    public static IOrderedEnumerable< CustomerOrder> GetCustomerOrdersByItemName(Data data, string itemName)
    {
      return
        from record in data.CustomerOrders
        where record.ItemName == itemName
        orderby record.Quantity
        select record;
    }

  }
}
