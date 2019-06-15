using ShipmentReconciliation.Properties;
using System;
using System.Configuration;
using System.Linq;

namespace ShipmentReconciliation
{
  internal class Program
  {
    private static Data _data;
    private static DataWrapper _dataWrapper;

    private enum Status { GenerateData, ProcessData, End}
    private static Status _nextState;
    private static void Main(string[] args)
    {
      //app name 
      Console.WriteLine(System.Reflection.Assembly.GetEntryAssembly().FullName);

      try
      {
        InitializeOptions(args);
        if (Settings.Default.Verbose)
        {
          DisplayOptions();
        }
        if (!Settings.Default.AutoStart)
        {
          Console.Write("Press 'Y' to continue, any other key to abort... ");
          char c = Console.ReadKey().KeyChar;
          Console.WriteLine();
          if (c != 'Y' && c != 'y')
          {
            Console.WriteLine("Operation aborted.");
            Settings.Default.AutoExit = true;
            return;
          }
        }
        RunStateMachine();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error: {ex.Message}");
      }
      finally
      {
        if (!Settings.Default.AutoExit)
        {
          Console.WriteLine("Press ENTER to exit...");
          Console.ReadLine();
        }
      }
    }

    private static void RunStateMachine()
    {
      while (_nextState != Status.End)
      {
        switch (_nextState)
        {
          case Status.GenerateData:
            if (Settings.Default.GenerateData)
            {
              GenerateData();
            }
            break;
          case Status.ProcessData:
            if (Settings.Default.ProcessData)
            {
              ProcessData();
            }
            break;
          default:
            return;
        }
      }
    }

    private static void InitializeOptions(string[] args)
    {
      var options = Settings.Default.Properties.Cast<SettingsProperty>();
      foreach (string item in args)
      {
        string[] parts = item.Split("=".ToCharArray(), 2);
        string settingName = parts[0];
        SettingsProperty setting = options.FirstOrDefault(p => p.Name == settingName);
        if (setting == null)
        {
          throw new ShipmentReconciliationException($"Invalid command line argument: {settingName}");
        }
        if (parts.Length == 1)
        {
          if (setting.PropertyType == typeof(bool))
          { Settings.Default[settingName] = true; }
          else
          { throw new ShipmentReconciliationException($"Missing value assignment of command line argument: {settingName}. Please use the form: Name=Value."); }
        }
        else if (parts.Length == 2)
        {
          Settings.Default[settingName] = Convert.ChangeType(parts[1], setting.PropertyType);
        }
      }
    }

    private static void DisplayOptions()
    {
      var options = Settings.Default.Properties.OfType<SettingsProperty>().OrderBy(s =>s.Name);
      
      Console.WriteLine("Settings:");
      foreach (var setting in options)
      {        
        Console.WriteLine($"\t{setting.Name}:\t{Settings.Default.PropertyValues[setting.Name].PropertyValue:N0}");
      }
    }

    private static void ProcessData()
    {
      Console.WriteLine("Loading data...");
      _data = DataLoader.LoadFolder("DATA\\Test3");
      Console.WriteLine("Loading data finished.");
      _dataWrapper = new DataWrapper(_data);
    }

    /// <summary>
    /// Generate test data
    /// </summary>
    private static void GenerateData()
    {
      while (true)
      {
        Console.WriteLine("Generating data...");
        _data = DataGenerator.Generate(
            maxNumberOfProducts: Settings.Default.GenerateDataMaxNumberOfProducts,
            maxNumberOfOrders: Settings.Default.GenerateDataMaxNumberOfOrders,
            maxNumberOfCustomers: Settings.Default.GenerateDataMaxNumberOfCustomers,
            maxQuantityPerOrder: Settings.Default.GenerateDataMaxQuantityPerOrder,
            maxTotalQuantityPerProduct: Settings.Default.GenerateDataMaxQuantityPerProduct);
        Console.WriteLine("Generating data finished.");

        _dataWrapper = new DataWrapper(_data);
        DisplaySummary(_dataWrapper);
        if (!Settings.Default.GenerateDataUserInteractive)
        { break; }

      }
      Console.WriteLine("Saving data...");
      DataSaver.SaveToFolder(_data, "DATA\\Test3");
      Console.WriteLine("Saving data finished.");
    }

    private static void DisplaySummary(DataWrapper dataWrapper, string title = "Summary:", string indent = "\t")
    {
      Console.WriteLine($"{title}");
      Console.WriteLine($"{indent}Factory Shipment: {dataWrapper.CountItemFactoryShipment:N0} items in {dataWrapper.CountRecordFactoryShipment:N0} shipments of {dataWrapper.CountProductFactoryShipment:N0} products.");
      Console.WriteLine($"{indent}Customer Orders: {dataWrapper.CountItemCustomerOrders:N0} items in {dataWrapper.CountRecordCustomerOrders:N0} orders of {dataWrapper.CountProductCustomerOrders:N0} products.");
      Console.WriteLine($"{indent}Total Deficit: {dataWrapper.TotalDeficit:N0} items of {dataWrapper.CountProductDeficit:N0} products.");
      Console.WriteLine($"{indent}Total Surplus: {dataWrapper.TotalSurplus:N0} items of {dataWrapper.CountProductSurplus:N0} products.");

    }
  }
}
