using CsvHelper.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace ShipmentReconciliation
{
  public partial class DataFile
  {
    /// <summary>
    /// Reads csv files from source folder.
    /// </summary>
    /// <param name="folderPath">Root folder path. Relative or absolute.</param>
    /// <param name="searchOption">Root only or subfolders too.</param>
    /// <param name="customerOrdersSearchPattern">Customer Order file name pattern.</param>
    /// <param name="factoryShipmentsSearchPattern">Factory Shipment file name pattern.</param>
    /// <param name="customerOrdersCsvConfiguration">Csv configuration to parse Customer Order files</param>
    /// <param name="factoryShipmentsCsvConfiguration">Csv configuration to parse Factory Shipment files</param>
    /// <param name="progressChanged">Callback for progress report</param>
    /// <param name="operation">Title text for progress report</param>
    /// <returns></returns>
    public static Data Load(string folderPath, SearchOption searchOption = SearchOption.TopDirectoryOnly, string customerOrdersSearchPattern = "*CustomerOrder*.csv", string factoryShipmentsSearchPattern = "*FactoryShipment*.csv", Configuration customerOrdersCsvConfiguration = null, Configuration factoryShipmentsCsvConfiguration = null, System.Action<string> progressChanged = null, [CallerMemberName] string operation = "")
    {
      CheckFolderSearchPatterns(customerOrdersSearchPattern, factoryShipmentsSearchPattern);
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

    /// <summary>
    /// Reads one Customer Order csv file and one Factory Shipment csv file.
    /// </summary>
    /// <param name="customerOrdersFilePath">File system path of the Customer Order csv file.</param>
    /// <param name="factoryShipmentsFilePath">File system path of the Factory Shipment csv file.</param>
    /// <param name="customerOrdersCsvConfiguration">Csv configuration to parse Customer Order files</param>
    /// <param name="factoryShipmentsCsvConfiguration">Csv configuration to parse Factory Shipment files</param>
    /// <param name="progressChanged">Callback for progress report</param>
    /// <param name="operation">Title text for progress report</param>
    /// <returns></returns>
    public static Data Load(string customerOrdersFilePath, string factoryShipmentsFilePath, Configuration customerOrdersCsvConfiguration = null, Configuration factoryShipmentsCsvConfiguration = null, System.Action<string> progressChanged = null, [CallerMemberName] string operation = "")
    {
      progressChanged?.Invoke($"{operation} CustomerOrders...");
      IList<CustomerOrder> customerOrders = CsvFile.ReadFromFile<CustomerOrder>(customerOrdersFilePath, customerOrdersCsvConfiguration, progressChanged, operation, cntRecords: out int cntRecordCustomerOrders);
      progressChanged?.Invoke($"{operation} FactoryShipments...");
      IList<FactoryShipment> factoryShipments = CsvFile.ReadFromFile<FactoryShipment>(factoryShipmentsFilePath, factoryShipmentsCsvConfiguration, progressChanged, operation, cntRecords: out int cntRecordFactoryShipments);
      Data inputData = new Data
      {
        CustomerOrders = customerOrders,
        FactoryShipments = factoryShipments
      };
      progressChanged?.Invoke($"{operation} CustomerOrder: {cntRecordCustomerOrders:N0} records, FactoryShipment: {cntRecordFactoryShipments:N0} records.");
      return inputData;
    }

    public static void CheckLoadParams(string folderPath, string filePathCustomerOrders, string filePathFactoryShipment, string customerOrdersSearchPattern, string factoryShipmentsSearchPattern)
    {
      CheckLoadPaths(folderPath, filePathCustomerOrders, filePathFactoryShipment);
      if (!string.IsNullOrEmpty(folderPath) && (string.IsNullOrEmpty(filePathCustomerOrders) || string.IsNullOrEmpty(filePathFactoryShipment)))
      {
        CheckFolderSearchPatterns(customerOrdersSearchPattern, factoryShipmentsSearchPattern);
      }
    }
    

    private static void CheckLoadPaths(string folderPath, string filePathCustomerOrders, string filePathFactoryShipment)
    {
      if (string.IsNullOrEmpty(folderPath) && (string.IsNullOrEmpty(filePathCustomerOrders) || string.IsNullOrEmpty(filePathFactoryShipment)))      
      {
        throw new ShipmentReconciliationException("Missing source path.");
      }
    }

    private static void CheckFolderSearchPatterns(string customerOrdersSearchPattern, string factoryShipmentsSearchPattern)
    {
      string errorMessage = string.Empty;
      if (string.IsNullOrEmpty(customerOrdersSearchPattern))
      { errorMessage += "CustomerOrders"; }
      if (string.IsNullOrEmpty(factoryShipmentsSearchPattern))
      {
        if (errorMessage.Length > 0)
        {
          errorMessage += ", ";
        }
        errorMessage += "FactoryShipment";
      }
      if (!string.IsNullOrEmpty(errorMessage))
      { throw new ShipmentReconciliationException("Missing file search pattern for: " + errorMessage); }
    }

    private static IList<T> LoadFromFolder<T>(string folderPath, string searchPattern, SearchOption searchOption, Configuration csvConfiguration, System.Action<string> progressChanged, string operation, out int cntFile, out int cntRecordsTotal)
    {
      string progressTitle = $"{operation} {nameof(LoadFromFolder)} {folderPath.TrimPath()} {searchPattern}";
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
        IEnumerable<T> records = CsvFile.ReadFromFile<T>(file, csvConfiguration, progressChanged, operation, out int cntRecords);
        cache.AddRange(records);
        cntRecordsTotal += cntRecords;
        progressChanged?.Invoke($"{progressTitle} {cntRecordsTotal:N0}");
      }
      return cache;
    }

  }
}
