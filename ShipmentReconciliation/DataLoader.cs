using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.IO;

namespace ShipmentReconciliation
{
  internal static class DataLoader 
  {
    public static Configuration DefaultConfiguration { get; set; } = new Configuration();

    public static Data LoadFolder(string folderPath, SearchOption searchOption = SearchOption.TopDirectoryOnly, string customerOrdersSearchPattern = "*CustomerOrder*.csv", Configuration customerOrdersCsvConfiguration = null, string factoryShipmentsSearchPattern = "*FactoryShipment*.csv", Configuration factoryShipmentsCsvConfiguration = null)
    {
      Data inputData = new Data
      {
        CustomerOrders = LoadCustomerOrdersFromFolder(folderPath, searchOption, customerOrdersSearchPattern, customerOrdersCsvConfiguration),
        FactoryShipments = LoadFactoryShipmentsFromFolder(folderPath, searchOption, factoryShipmentsSearchPattern, factoryShipmentsCsvConfiguration)
      };
      return inputData;
    }

    public static Data LoadFiles(string customerOrdersFilePath, string factoryShipmentsFilePath, Configuration customerOrdersCsvConfiguration = null, Configuration factoryShipmentsCsvConfiguration = null)
    {
      Data inputData = new Data
      {
        CustomerOrders = LoadCustomerOrdersFromFile(customerOrdersFilePath, customerOrdersCsvConfiguration),
        FactoryShipments = LoadFactoryShipmentsFromFile(factoryShipmentsFilePath, factoryShipmentsCsvConfiguration)
      };
      return inputData;
    }

    public static IList<CustomerOrder> LoadCustomerOrdersFromFile(string filePath, Configuration csvConfiguration = null)
    {
      List<CustomerOrder> cache = new List<CustomerOrder>();
      using (StreamReader reader = new StreamReader(filePath))
      using (CsvHelper.CsvReader csv = new CsvHelper.CsvReader(reader, csvConfiguration ?? DefaultConfiguration))
      {
        csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.Replace(" ", "");
        IEnumerable<CustomerOrder> records = csv.GetRecords<CustomerOrder>();
        cache.AddRange(records);
      }
      return cache;
    }

    public static IList<FactoryShipment> LoadFactoryShipmentsFromFile(string filePath, Configuration csvConfiguration = null)
    {
      List<FactoryShipment> cache = new List<FactoryShipment>();
      using (StreamReader reader = new StreamReader(filePath))
      using (CsvHelper.CsvReader csv = new CsvHelper.CsvReader(reader, csvConfiguration ?? DefaultConfiguration))
      {
        csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.Replace(" ", "");
        IEnumerable<FactoryShipment> records = csv.GetRecords<FactoryShipment>();
        cache.AddRange(records);
      }
      return cache;
    }

    public static IList<CustomerOrder> LoadCustomerOrdersFromFolder(string folderPath, SearchOption searchOption = SearchOption.TopDirectoryOnly, string customerOrdersSearchPattern = "*CustomerOrder*.csv", Configuration csvConfiguration = null)
    {
      string[] files = Directory.GetFiles(folderPath, customerOrdersSearchPattern, searchOption);
      List<CustomerOrder> customerOrders = new List<CustomerOrder>();
      foreach (string file in files)
      {
        IEnumerable<CustomerOrder> records = LoadCustomerOrdersFromFile(file, csvConfiguration);
        customerOrders.AddRange(records);
      }
      return customerOrders;
    }

    public static IList<FactoryShipment> LoadFactoryShipmentsFromFolder(string folderPath, SearchOption searchOption = SearchOption.TopDirectoryOnly, string FactoryShipmentsSearchPattern = "*FactoryShipment*.csv", Configuration csvConfiguration = null)
    {
      string[] files = Directory.GetFiles(folderPath, FactoryShipmentsSearchPattern, searchOption);
      List<FactoryShipment> FactoryShipments = new List<FactoryShipment>();
      foreach (string file in files)
      {
        IEnumerable<FactoryShipment> records = LoadFactoryShipmentsFromFile(file, csvConfiguration);
        FactoryShipments.AddRange(records);
      }
      return FactoryShipments;
    }
  }
}
