using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.IO;

namespace ShipmentReconciliation
{
  partial class DataFile
  {    
    public static void Save(Data data, string folderPath, string customerOrdersFileName = "CustomerOrders.csv", string factoryShipmentsFileName = "FactoryShipments.csv", Configuration customerOrdersCsvConfiguration = null, Configuration factoryShipmentsCsvConfiguration = null)
    {
      CheckFolder(folderPath);
      WriteToFile(data.CustomerOrders, Path.Combine(folderPath, customerOrdersFileName), customerOrdersCsvConfiguration);
      WriteToFile(data.FactoryShipments, Path.Combine(folderPath, factoryShipmentsFileName), factoryShipmentsCsvConfiguration);
    }

    public static void Save<T>(IEnumerable<T> records, string folderPath, string fileName, Configuration csvConfiguration = null)
    {
      CheckFolder(folderPath);
      WriteToFile(records, Path.Combine(folderPath, fileName), csvConfiguration);
    }

    public static void Save<T>(IEnumerable<T> records, string filePath, Configuration csvConfiguration = null)
    {
      CheckFolder(Path.GetDirectoryName(filePath));
      WriteToFile(records, filePath, csvConfiguration);
    }
    
    private static void CheckFolder(string folderPath)
    {
      if (!Directory.Exists(folderPath))
      { Directory.CreateDirectory(folderPath); }
    }

    private static void WriteToFile<T>(IEnumerable<T> records, string filePath, Configuration csvConfiguration = null)
    {
      using (StreamWriter writer = new StreamWriter(filePath))
      using (CsvWriter csv = new CsvWriter(writer, csvConfiguration ?? DefaultConfiguration))
      {
        csv.WriteRecords(records);
      }
    }
    
  }
}
