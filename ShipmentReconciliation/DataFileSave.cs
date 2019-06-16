using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace ShipmentReconciliation
{
  internal partial class DataFile
  {
    public static void Save(Data data, string folderPath, string customerOrdersFileName = "CustomerOrders.csv", string factoryShipmentsFileName = "FactoryShipments.csv", Configuration customerOrdersCsvConfiguration = null, Configuration factoryShipmentsCsvConfiguration = null, System.Action<string> progressChanged = null, [CallerMemberName] string operation = "")
    {
      CheckFolder(folderPath, progressChanged, operation);
      int cntFile = 0; int allFiles = 2;
      WriteToFile(data.CustomerOrders, Path.Combine(folderPath, customerOrdersFileName), customerOrdersCsvConfiguration, progressChanged, progressOperation: $"{operation} {++cntFile}/{allFiles}");
      WriteToFile(data.FactoryShipments, Path.Combine(folderPath, factoryShipmentsFileName), factoryShipmentsCsvConfiguration, progressChanged, progressOperation: $"{operation} {++cntFile}/{allFiles}");
    }

    public static void Save<T>(IEnumerable<T> records, string folderPath, string fileName, Configuration csvConfiguration = null, System.Action<string> progressChanged = null, int cntFile = 1, int allFiles = 1, [CallerMemberName] string operation = "")
    {
      CheckFolder(folderPath, progressChanged, operation);
      WriteToFile(records, Path.Combine(folderPath, fileName), csvConfiguration, progressChanged, operation);
    }

    public static void Save<T>(IEnumerable<T> records, string filePath, Configuration csvConfiguration = null, System.Action<string> progressChanged = null, int cntFile = 1, int allFiles = 1, [CallerMemberName] string operation = "")
    {
      CheckFolder(Path.GetDirectoryName(filePath), progressChanged, operation);
      WriteToFile(records, filePath, csvConfiguration, progressChanged, operation);
    }

    private static void CheckFolder(string path, System.Action<string> progressChanged, string progressOperation)
    {
      progressChanged?.Invoke($"{progressOperation} {nameof(CheckFolder)} {Trim(path)}");
      if (!Directory.Exists(path))
      { Directory.CreateDirectory(path); }
    }

    private static void WriteToFile<T>(IEnumerable<T> records, string path, Configuration csvConfiguration, System.Action<string> progressChanged, string progressOperation)
    {
      progressOperation = $"{progressOperation} {nameof(WriteToFile)} {Trim(path)}";
      progressChanged?.Invoke(progressOperation);
      using (StreamWriter writer = new StreamWriter(path))
      using (CsvWriter csv = new CsvWriter(writer, csvConfiguration ?? DefaultConfiguration))
      {
        if (progressChanged == null)
        { csv.WriteRecords(records); }
        else
        {
          csv.WriteHeader<T>();
          int cntRecords = ProcessRecords(records, (item) => { csv.NextRecord(); csv.WriteRecord(item); }, progressChanged, progressOperation);
          progressChanged?.Invoke($"{progressOperation} {cntRecords} items.");
        }

      }
    }

  }
}
