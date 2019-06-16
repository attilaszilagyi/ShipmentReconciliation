using CsvHelper.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace ShipmentReconciliation
{
  internal partial class DataFile
  {
    public static Data Load(string folderPath, SearchOption searchOption = SearchOption.TopDirectoryOnly, string customerOrdersSearchPattern = "*CustomerOrder*.csv", string factoryShipmentsSearchPattern = "*FactoryShipment*.csv", Configuration customerOrdersCsvConfiguration = null, Configuration factoryShipmentsCsvConfiguration = null, System.Action<string> progressChanged = null, [CallerMemberName] string operation = "")
    {
      progressChanged?.Invoke($"{operation} CustomerOrders...");
      IList<CustomerOrder> customerOrders = LoadFromFolder<CustomerOrder>(folderPath, customerOrdersSearchPattern, searchOption, customerOrdersCsvConfiguration, progressChanged, operation, out int cntFileCustomerOrders, out int cntRecordCustomerOrders);
      progressChanged?.Invoke($"{operation} FactoryShipments...");
      IList<FactoryShipment> factoryShipments = LoadFromFolder<FactoryShipment>(folderPath, factoryShipmentsSearchPattern, searchOption, factoryShipmentsCsvConfiguration, progressChanged, operation, out int cntFileFactoryShipments, out int cntRecordFactoryShipments);
      Data inputData = new Data
      {
        CustomerOrders = customerOrders,
        FactoryShipments = factoryShipments
      };
      progressChanged?.Invoke($"{operation} CustomerOrder: {cntRecordCustomerOrders:N0} records in {cntFileCustomerOrders:N0} files, FactoryShipment: {cntRecordFactoryShipments:N0} records in {cntFileFactoryShipments:N0} files.");
      return inputData;
    }

    public static Data Load(string customerOrdersFilePath, string factoryShipmentsFilePath, Configuration customerOrdersCsvConfiguration = null, Configuration factoryShipmentsCsvConfiguration = null, System.Action<string> progressChanged = null, [CallerMemberName] string operation = "")
    {
      progressChanged?.Invoke($"{operation} CustomerOrders...");
      IList<CustomerOrder> customerOrders = LoadFromFile<CustomerOrder>(customerOrdersFilePath, customerOrdersCsvConfiguration, progressChanged, operation, cntRecords: out int cntRecordCustomerOrders);
      progressChanged?.Invoke($"{operation} FactoryShipments...");
      IList<FactoryShipment> factoryShipments = LoadFromFile<FactoryShipment>(factoryShipmentsFilePath, factoryShipmentsCsvConfiguration, progressChanged, operation, cntRecords: out int cntRecordFactoryShipments);
      Data inputData = new Data
      {
        CustomerOrders = customerOrders,
        FactoryShipments = factoryShipments
      };
      progressChanged?.Invoke($"{operation} CustomerOrder: {cntRecordCustomerOrders:N0} records, FactoryShipment: {cntRecordFactoryShipments:N0} records.");
      return inputData;
    }

    private static IList<T> LoadFromFolder<T>(string folderPath, string searchPattern, SearchOption searchOption, Configuration csvConfiguration, System.Action<string> progressChanged, string operation, out int cntFile, out int cntRecordsTotal)
    {
      string progressTitle = $"{operation} {nameof(LoadFromFolder)} {Trim(folderPath)} {searchPattern}";
      progressChanged?.Invoke(progressTitle);
      string[] files = Directory.GetFiles(folderPath, searchPattern, searchOption);
      List<T> cache = new List<T>();
      int allFiles = files.Length;
      cntFile = 0;
      cntRecordsTotal = 0;
      foreach (string file in files)
      {
        cntFile++;
        progressChanged?.Invoke($"{progressTitle} {cntFile:N0}/{allFiles:N0}");
        IEnumerable<T> records = LoadFromFile<T>(file, csvConfiguration, progressChanged, operation, out int cntRecords);
        cache.AddRange(records);
        cntRecordsTotal += cntRecords;
        progressChanged?.Invoke($"{progressTitle} {cntRecordsTotal:N0}");
      }
      return cache;
    }

    private static IList<T> LoadFromFile<T>(string filePath, Configuration csvConfiguration, System.Action<string> progressChanged, string operation, out int cntRecords)
    {
      string progressTitle = $"{operation} {nameof(LoadFromFile)} {Trim(filePath)}";
      progressChanged?.Invoke(progressTitle);
      List<T> cache = new List<T>();
      using (StreamReader reader = new StreamReader(filePath))
      using (CsvHelper.CsvReader csv = new CsvHelper.CsvReader(reader, csvConfiguration ?? DefaultConfiguration))
      {
        csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.Replace(" ", "");
        IEnumerable<T> records = csv.GetRecords<T>();
        if (progressChanged == null)
        { cache.AddRange(records); cntRecords = cache.Count; }
        else
        {
          cntRecords = ProcessRecords(records, (item) => { cache.Add(item); }, progressChanged, progressTitle);
          progressChanged($"{progressTitle} {cntRecords:N0}");
        }
      }
      return cache;
    }

  }
}
