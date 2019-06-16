using CsvHelper.Configuration;
using System.Collections.Generic;
using System.IO;

namespace ShipmentReconciliation
{
  partial class DataFile
  {    
    public static Data Load(string folderPath, SearchOption searchOption = SearchOption.TopDirectoryOnly, string customerOrdersSearchPattern = "*CustomerOrder*.csv", string factoryShipmentsSearchPattern = "*FactoryShipment*.csv", Configuration customerOrdersCsvConfiguration = null, Configuration factoryShipmentsCsvConfiguration = null, System.Action<string> progressChanged = null)
    {
      Data inputData = new Data
      {
        CustomerOrders = LoadFromFolder<CustomerOrder>(folderPath, customerOrdersSearchPattern, searchOption, customerOrdersCsvConfiguration, progressChanged),
        FactoryShipments = LoadFromFolder<FactoryShipment>(folderPath, factoryShipmentsSearchPattern, searchOption, factoryShipmentsCsvConfiguration, progressChanged)
      };
      return inputData;
    }

    public static Data Load(string customerOrdersFilePath, string factoryShipmentsFilePath, Configuration customerOrdersCsvConfiguration = null, Configuration factoryShipmentsCsvConfiguration = null, System.Action<string> progressChanged = null)
    {
      Data inputData = new Data
      {
        CustomerOrders = LoadFromFile<CustomerOrder>(customerOrdersFilePath, customerOrdersCsvConfiguration, progressChanged),
        FactoryShipments = LoadFromFile<FactoryShipment>(factoryShipmentsFilePath, factoryShipmentsCsvConfiguration, progressChanged)
      };
      return inputData;
    }

    private static IList<T> LoadFromFolder<T>(string folderPath, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly, Configuration csvConfiguration = null, System.Action<string> progressChanged = null)
    {
      string[] files = Directory.GetFiles(folderPath, searchPattern, searchOption);
      List<T> cache = new List<T>();
      int fileAll = files.Length;
      int fileCnt = 0;
      foreach (string file in files)
      {
        fileCnt++;
        IEnumerable<T> records = LoadFromFile<T>(file, csvConfiguration, progressChanged, fileCnt, fileAll);
        cache.AddRange(records);
      }
      return cache;
    }

    private static IList<T> LoadFromFile<T>(string filePath, Configuration csvConfiguration = null, System.Action<string> progressChanged = null, int cntFile = 1, int allFiles = 1)
    {
      List<T> cache = new List<T>();
      using (StreamReader reader = new StreamReader(filePath))
      using (CsvHelper.CsvReader csv = new CsvHelper.CsvReader(reader, csvConfiguration ?? DefaultConfiguration))
      {
        csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.Replace(" ", "");
        IEnumerable<T> records = csv.GetRecords<T>();
        if (progressChanged == null)
        { cache.AddRange(records); }
        else
        {
          AddRecords(cache, records, cntFile, allFiles, filePath, progressChanged);
        }
      }
      return cache;
    }

    private static void AddRecords<T>(List<T> cache, IEnumerable<T> records, int cntFile, int allFiles, string filePath, System.Action<string> progressChanged)
    {
      int cntRecord = 0;
      string fileName = filePath; if (fileName.Length > 50)
      {
        fileName = "..." + fileName.Substring(fileName.Length - 47);
      }

      foreach (T item in records)
      {
        cache.Add(item);
        cntRecord++;
        if (cntRecord % 100 == 0)
        {
          progressChanged($"{cntFile}/{allFiles} {fileName} {cntRecord}");
        }
      }
    }


  }
}
