using CsvHelper.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace ShipmentReconciliation
{
  public partial class DataFile
  {
    /// <summary>
    /// Saves Customer Order and Factory Shipment records to csv files in the provided file system folder. 
    /// </summary>
    /// <param name="data">Records to save</param>
    /// <param name="folderPath">Destination folder path. Relative or absolute.</param>
    /// <param name="customerOrdersFileName">File name of the Customer Orders csv file.</param>
    /// <param name="factoryShipmentsFileName">File name ot the Factory Shipment csv file.</param>
    /// <param name="customerOrdersCsvConfiguration">Csv options of the Customer Orders csv file.</param>
    /// <param name="factoryShipmentsCsvConfiguration">Csv options of the Factory Shipment csv file.</param>
    /// <param name="progressChanged">Callback to report progress</param>
    /// <param name="operation">Progress report title text</param>
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

    public static void Save(IEnumerable<CustomerOrder> customerOrders, string customerOrdersFilePath, IEnumerable<FactoryShipment> factoryShipments, string factoryShipmentsFilePath, Configuration customerOrdersCsvConfiguration = null, Configuration factoryShipmentsCsvConfiguration = null, System.Action<string> progressChanged = null, [CallerMemberName] string operation = "")
    {
      CheckFolder(Path.GetDirectoryName(customerOrdersFilePath), progressChanged, operation);
      CheckFolder(Path.GetDirectoryName(factoryShipmentsFilePath), progressChanged, operation);
      SaveFiles(customerOrders, customerOrdersFilePath, factoryShipments, factoryShipmentsFilePath, customerOrdersCsvConfiguration, factoryShipmentsCsvConfiguration, progressChanged, operation);
    }

    private static void SaveFiles(IEnumerable<CustomerOrder> customerOrders, string customerOrdersFilePath, IEnumerable<FactoryShipment> factoryShipments, string factoryShipmentsFilePath, Configuration customerOrdersCsvConfiguration, Configuration factoryShipmentsCsvConfiguration, System.Action<string> progressChanged, string operation)
    {
      ProgressStatus status = new ProgressStatus(operation, progressChanged, 100);
      //!CsvWriter is NOT threadSafe!!!
      //Task.WaitAll(
      //    Task.Factory.StartNew(() => WriteToFile(customerOrders, customerOrdersFilePath, customerOrdersCsvConfiguration, (count, message) => { status.CustomerOrderMessage = message; status.CustomerOrderCount = count; })),
      //    Task.Factory.StartNew(() => WriteToFile(factoryShipments, factoryShipmentsFilePath, factoryShipmentsCsvConfiguration, (count, message) => { status.FactoryShipmentMessage = message; status.FactoryShipmentCount = count; }))
      //    );
      CsvFile.WriteToFile(customerOrders, customerOrdersFilePath, customerOrdersCsvConfiguration, (count, message) => { status.CustomerOrderMessage = message; status.CustomerOrderCount = count; }, operation);
      CsvFile.WriteToFile(factoryShipments, factoryShipmentsFilePath, factoryShipmentsCsvConfiguration, (count, message) => { status.FactoryShipmentMessage = message; status.FactoryShipmentCount = count; }, operation);
      status.Report();
    }

    private static void CheckFolder(string path, System.Action<string> progressChanged, string progressOperation)
    {
      progressChanged?.Invoke($"{progressOperation} {nameof(CheckFolder)} {path.TrimPath()}");
      if (!Directory.Exists(path))
      { Directory.CreateDirectory(path); }
    }


  }
}
