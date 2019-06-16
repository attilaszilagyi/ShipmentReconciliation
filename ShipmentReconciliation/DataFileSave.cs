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
      SaveFiles(
        data.CustomerOrders,
        Path.Combine(folderPath, customerOrdersFileName),
        data.FactoryShipments,
        Path.Combine(folderPath, factoryShipmentsFileName),
        customerOrdersCsvConfiguration,
        factoryShipmentsCsvConfiguration,
        progressChanged, operation
        );
    }
    public static void Save(IList<CustomerOrder> customerOrders, string customerOrdersFilePath, IList<FactoryShipment> factoryShipments, string factoryShipmentsFilePath, Configuration customerOrdersCsvConfiguration = null, Configuration factoryShipmentsCsvConfiguration = null, System.Action<string> progressChanged = null, [CallerMemberName] string operation = "")
    {
      CheckFolder(Path.GetDirectoryName(customerOrdersFilePath), progressChanged, operation);
      CheckFolder(Path.GetDirectoryName(factoryShipmentsFilePath), progressChanged, operation);
      SaveFiles(customerOrders, customerOrdersFilePath, factoryShipments, factoryShipmentsFilePath, customerOrdersCsvConfiguration, factoryShipmentsCsvConfiguration, progressChanged, operation);
    }

    private static void SaveFiles(IList<CustomerOrder> customerOrders, string customerOrdersFilePath, IList<FactoryShipment> factoryShipments, string factoryShipmentsFilePath, Configuration customerOrdersCsvConfiguration, Configuration factoryShipmentsCsvConfiguration, System.Action<string> progressChanged, string operation)
    {
      Status status = new Status(operation, progressChanged, 100);
      //!CsvWriter is NOT threadSafe!!!
      //Task.WaitAll(
      //    Task.Factory.StartNew(() => WriteToFile(customerOrders, customerOrdersFilePath, customerOrdersCsvConfiguration, (count, message) => { status.CustomerOrderMessage = message; status.CustomerOrderCount = count; })),
      //    Task.Factory.StartNew(() => WriteToFile(factoryShipments, factoryShipmentsFilePath, factoryShipmentsCsvConfiguration, (count, message) => { status.FactoryShipmentMessage = message; status.FactoryShipmentCount = count; }))
      //    );
      WriteToFile(customerOrders, customerOrdersFilePath, customerOrdersCsvConfiguration, (count, message) => { status.CustomerOrderMessage = message; status.CustomerOrderCount = count; });
      WriteToFile(factoryShipments, factoryShipmentsFilePath, factoryShipmentsCsvConfiguration, (count, message) => { status.FactoryShipmentMessage = message; status.FactoryShipmentCount = count; });
      status.Report();
    }

    private static void CheckFolder(string path, System.Action<string> progressChanged, string progressOperation)
    {
      progressChanged?.Invoke($"{progressOperation} {nameof(CheckFolder)} {Trim(path)}");
      if (!Directory.Exists(path))
      { Directory.CreateDirectory(path); }
    }

    private static void WriteToFile<T>(IEnumerable<T> records, string path, Configuration csvConfiguration, System.Action<int, string> progressChanged)
    {
      //progressChanged?.Invoke(0, Trim(path));
      using (StreamWriter writer = new StreamWriter(path))
      using (CsvWriter csv = new CsvWriter(writer, csvConfiguration ?? DefaultConfiguration))
      {
        if (progressChanged == null)
        { csv.WriteRecords(records); }
        else
        {
          csv.WriteHeader<T>();
          int cntRecords = ProcessRecords(records, (item) => { csv.NextRecord(); csv.WriteRecord(item); }, progressChanged, null);
          //progressChanged?.Invoke(cntRecords, Trim(path));
        }

      }
    }

  }
}
