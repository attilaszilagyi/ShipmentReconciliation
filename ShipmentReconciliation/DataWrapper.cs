using System.Collections.Generic;
using System.Linq;

namespace ShipmentReconciliation
{
  /// <summary>
  /// Provides pre-processed input data: Customer Orders grouped by Item Name and Summed Quantities grouped by Item Name.
  /// </summary>
  internal class DataWrapper
  {
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

    public int TotalSurplus { get; private set; }
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

    public void Recalculate()
    {
      SumFactoryShipments = CalculateSum(GetFactoryShipmentsByItemName());
      SumCustomerOrders = CalculateSum(GetCustomerOrdersByItemName());
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

    public IOrderedEnumerable<IGrouping<string, CustomerOrder>> GetCustomerOrdersByItemName()
    {
      return GetCustomerOrdersByItemName(Data);
    }

    public static IOrderedEnumerable<IGrouping<string, CustomerOrder>> GetCustomerOrdersByItemName(Data data)
    {
      return
        from record in data.CustomerOrders
        group record by record.ItemName into itemNameGroup
        orderby itemNameGroup.Key
        select itemNameGroup;
    }

    public IOrderedEnumerable<IGrouping<string, FactoryShipment>> GetFactoryShipmentsByItemName()
    {
      return GetFactoryShipmentsByItemName(Data);
    }

    public static IOrderedEnumerable<IGrouping<string, FactoryShipment>> GetFactoryShipmentsByItemName(Data data)
    {
      return
       from record in data.FactoryShipments
       group record by record.ItemName into itemNameGroup
       orderby itemNameGroup.Key
       select itemNameGroup;
    }
  }
}
