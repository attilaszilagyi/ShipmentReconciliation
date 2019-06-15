using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.IO;

namespace ShipmentReconciliation
{
  internal static class DataSaver
  {
    public static Configuration DefaultConfiguration { get; set; } = new Configuration();

    public static void SaveToFolder(Data data, string folderPath, string customerOrdersFileName = "CustomerOrders.csv", Configuration customerOrdersCsvConfiguration = null, string factoryShipmentsFileName = "FactoryShipments.csv", Configuration factoryShipmentsCsvConfiguration = null)
    {
      if (!Directory.Exists(folderPath))
      { Directory.CreateDirectory(folderPath); }
      saveToFile(data.CustomerOrders, Path.Combine(folderPath, customerOrdersFileName), customerOrdersCsvConfiguration);
      saveToFile(data.FactoryShipments, Path.Combine(folderPath, factoryShipmentsFileName), factoryShipmentsCsvConfiguration);
    }

    public static void SaveToFolder(IEnumerable<CustomerOrder> records, string folderPath, string fileName = "CustomerOrders.csv", Configuration csvConfiguration = null)
    {
      if (!Directory.Exists(folderPath))
      { Directory.CreateDirectory(folderPath); }
      saveToFile(records, Path.Combine(folderPath, fileName), csvConfiguration);
    }

    public static void SaveToFolder(IEnumerable<FactoryShipment> records, string folderPath, string fileName = "FactoryShipments.csv", Configuration csvConfiguration = null)
    {
      checkFolder(folderPath);
      saveToFile(records, Path.Combine(folderPath, fileName), csvConfiguration);
    }

    private static void checkFolder(string folderPath)
    {
      if (!Directory.Exists(folderPath))
      { Directory.CreateDirectory(folderPath); }
    }

    public static void SaveToFile(IEnumerable<CustomerOrder> records, string filePath, Configuration csvConfiguration = null)
    {
      checkFolder(Path.GetDirectoryName(filePath));
      using (StreamWriter writer = new StreamWriter(filePath))
      using (CsvWriter csv = new CsvWriter(writer, csvConfiguration ?? DefaultConfiguration))
      {
        csv.WriteRecords(records);
      }
    }

    public static void SaveToFile(IEnumerable<FactoryShipment> records, string filePath, Configuration csvConfiguration = null)
    {
      checkFolder(Path.GetDirectoryName(filePath));
      using (StreamWriter writer = new StreamWriter(filePath))
      using (CsvWriter csv = new CsvWriter(writer, csvConfiguration ?? DefaultConfiguration))
      {
        csv.WriteRecords(records);
      }
    }

    public static void saveToFile(IEnumerable<CustomerOrder> records, string filePath, Configuration csvConfiguration = null)
    {
      using (StreamWriter writer = new StreamWriter(filePath))
      using (CsvWriter csv = new CsvWriter(writer, csvConfiguration ?? DefaultConfiguration))
      {
        csv.WriteRecords(records);
      }
    }

    public static void saveToFile(IEnumerable<FactoryShipment> records, string filePath, Configuration csvConfiguration = null)
    {
      using (StreamWriter writer = new StreamWriter(filePath))
      using (CsvWriter csv = new CsvWriter(writer, csvConfiguration ?? DefaultConfiguration))
      {
        csv.WriteRecords(records);
      }
    }
  }
}
