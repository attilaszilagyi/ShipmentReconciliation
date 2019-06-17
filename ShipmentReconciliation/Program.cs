using ShipmentReconciliation.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace ShipmentReconciliation
{
  public class Program
  {
    /// <summary>
    /// 1., Generates input data and stores to csv files and/or 
    /// 2., Loads input data from csv files and 
    /// 3., Processes input data, reconciliating shipments with orders 
    /// (optimization with simple greedy algorithm and 01-knapsack problem solver minimizes surplus to store) and
    /// 4., Lists result on screen and/or saves them to file.
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {

      DisplayTitle();
      try
      {
        LoadSettings(args);
        DisplaySettings();
        ValidateSettings();
        if (PromptStart())
        {
          if (Settings.Default.GenerateData)
          {
            GenerateData();
            ValidateData();
            SaveData();
          }
          if (Settings.Default.ProcessData)
          {
            LoadData();
            ValidateData();
            ProcessData();
            DisplayResult();
            SaveResult();
          }
        }
      }
      catch (ShipmentReconciliationException ex)
      {
        HandleWarning(ex);
      }
      catch (Exception ex)
      {
        HandleError(ex);
      }
      finally
      {
        PromptFinish();
      }
    }

    /// <summary>
    /// Input data, randomly generated or loaded from file system. Customer Order and Factory Shipment records.
    /// </summary>
    private static Data _data;
    /// <summary>
    /// Pre-processed input data (sums, counts, balance) and helper functions to get input data records grouped and ordered. 
    /// </summary>
    private static DataWrapper _dataWrapper;
    /// <summary>
    /// Optimization process result. Orders to fulfill, products to store.
    /// </summary>
    private static Result _result;
    /// <summary>
    /// Helper anonym to display progress report messages on screen.
    /// </summary>
    private static readonly System.Action<string> progressChanged = text => { Console.Write("\r" + text.PadRight(100)); };

    private static readonly string _subFolderID = DateTime.Now.Ticks.ToString();

    /// <summary>
    /// Welcome message
    /// </summary>
    private static void DisplayTitle()
    {
      Console.WriteLine(System.Reflection.Assembly.GetEntryAssembly().FullName);
    }

    /// <summary>
    /// Parse command line arguments as application settings
    /// </summary>
    /// <param name="args"></param>
    private static void LoadSettings(string[] args)
    {
      System.Collections.Generic.IEnumerable<SettingsProperty> settings = Settings.Default.Properties.Cast<SettingsProperty>();
      foreach (string item in args)
      {
        string[] parts = item.Split("=".ToCharArray(), 2);
        string settingName = parts[0];
        SettingsProperty setting = settings.FirstOrDefault(p => p.Name == settingName);
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
          string settingValue = parts[1].TrimStart('"').TrimEnd('"');
          Settings.Default[settingName] = Convert.ChangeType(settingValue, setting.PropertyType);
        }
      }

      CsvHelper.Configuration.Configuration csvConfiguration = !string.IsNullOrEmpty(Settings.Default.CsvConfigurationCulture) ? new CsvHelper.Configuration.Configuration(new System.Globalization.CultureInfo(Settings.Default.CsvConfigurationCulture)) : new CsvHelper.Configuration.Configuration();
      if (!string.IsNullOrEmpty(Settings.Default.CsvConfigurationDelimiter))
      {
        csvConfiguration.Delimiter = Settings.Default.CsvConfigurationDelimiter;
      }

      if (!string.IsNullOrEmpty(Settings.Default.CsvConfigurationEncoding))
      {
        csvConfiguration.Encoding = System.Text.Encoding.GetEncoding(Settings.Default.CsvConfigurationEncoding);
      }

      CsvFile.DefaultConfiguration = csvConfiguration;
    }

    /// <summary>
    /// List all setting values to use for this run
    /// </summary>
    private static void DisplaySettings()
    {
      if (!Settings.Default.Verbose)
      { return; }
      IOrderedEnumerable<SettingsProperty> settings = Settings.Default.Properties.OfType<SettingsProperty>().OrderBy(s => s.Name);

      Console.WriteLine($"{nameof(DisplaySettings)}");
      foreach (SettingsProperty setting in settings)
      {
        Console.WriteLine($"\t{setting.Name}:\t{Settings.Default.PropertyValues[setting.Name].PropertyValue:N0}");
      }
    }

    /// <summary>
    /// Checks application setting / command line argument values.
    /// </summary>
    private static void ValidateSettings()
    {
      Console.Write($"{nameof(ValidateSettings)} ... ");

      if (Settings.Default.ProcessData && !Settings.Default.GenerateData)
      { DataFile.CheckLoadParams(Settings.Default.FolderPath, Settings.Default.FilePathCustomerOrders, Settings.Default.FilePathFactoryShipment, Settings.Default.FolderSearchPatternCustomerOrders, Settings.Default.FolderSearchPatternFactoryShipment); }
      Console.Write($"\r{nameof(ValidateSettings)}     ");
    }

    /// <summary>
    /// Prompt user to start the application
    /// </summary>
    /// <returns></returns>
    private static bool PromptStart()
    {
      if (Settings.Default.GenerateData || Settings.Default.ProcessData)
      {
        string operation = string.Empty;
        if (Settings.Default.GenerateData)
        {
          operation += $"{nameof(GenerateData)} ";
          if (!string.IsNullOrEmpty(Settings.Default.FolderPath))
          {
            Console.WriteLine($"Subfolder: {_subFolderID}");
          }
          else if (string.IsNullOrEmpty(Settings.Default.FilePathCustomerOrders) || string.IsNullOrEmpty(Settings.Default.FilePathFactoryShipment))
          {//no destination path provided (=> we don't save data to file system)
            Console.WriteLine("No file/folder paths provided to save generated data.");
          }
        }

        if (Settings.Default.ProcessData)
        {
          operation += $"{nameof(ProcessData)} ";

          if (string.IsNullOrEmpty(Settings.Default.ResultFileNameFulfill))
          { Console.WriteLine("No filename provided where to save records of Customer Orders to fulfill."); }

          if (!(Path.IsPathRooted(Settings.Default.ResultFileNameFulfill)) && string.IsNullOrEmpty(Settings.Default.ResultFolderPath) && string.IsNullOrEmpty(Settings.Default.FolderPath))
          { Console.WriteLine("No file/folder path provided where to save records of Customer Orders to fulfill."); }

          if (string.IsNullOrEmpty(Settings.Default.ResultFileNameStore))
          { Console.WriteLine("No filename provided where to save records of surplus product quantities to store."); }

          if (!(Path.IsPathRooted(Settings.Default.ResultFileNameStore)) && string.IsNullOrEmpty(Settings.Default.ResultFolderPath) && string.IsNullOrEmpty(Settings.Default.FolderPath))
          { Console.WriteLine("No file/folder path provided where to save records of surplus product quantities to store."); }

        }

        Console.WriteLine("Operation: " + operation);


        if (!Settings.Default.AutoStart)
        {
          Console.Write("Press 'Y' to continue, any other key to abort... ");
          char c = Console.ReadKey().KeyChar;
          Console.WriteLine();
          if (c != 'Y' && c != 'y')
          {
            Console.WriteLine("Operation aborted.");
            Settings.Default.AutoExit = true;
            return false;
          }
        }
      }
      else
      {
        Console.WriteLine($"No operation ({nameof(GenerateData)} or {nameof(ProcessData)}) selected to run.");
        return false;
      }
      return true;
    }

    /// <summary>
    /// Generate test data
    /// </summary>
    private static void GenerateData()
    {
      Console.Write($"{nameof(GenerateData)} ... ");
      _data = DataGenerator.Generate(
          maxNumberOfProducts: Settings.Default.GenerateDataMaxNumberOfProducts,
          maxNumberOfOrders: Settings.Default.GenerateDataMaxNumberOfOrders,
          maxNumberOfCustomers: Settings.Default.GenerateDataMaxNumberOfCustomers,
          maxQuantityPerOrder: Settings.Default.GenerateDataMaxQuantityPerOrder,
          maxTotalQuantityPerProduct: Settings.Default.GenerateDataMaxQuantityPerProduct,
          progressChanged: progressChanged);
      Console.WriteLine();
    }

    /// <summary>
    /// Saves test data to the file system.
    /// </summary>
    /// <remarks>Creates a uniquely named subfolder in the target directory.</remarks>
    private static void SaveData()
    {
      Console.Write($"{nameof(SaveData)} ... ");
      //Destination folder path provided
      if (!string.IsNullOrEmpty(Settings.Default.FolderPath))
      {
        string folderPath = System.IO.Path.Combine(Settings.Default.FolderPath, _subFolderID);
        Console.Write(folderPath);
        DataFile.Save(_data, folderPath,
            progressChanged: progressChanged);
        Settings.Default.FolderPath = folderPath;

      }
      //Destination file names are provided
      else if (!string.IsNullOrEmpty(Settings.Default.FilePathCustomerOrders) && !string.IsNullOrEmpty(Settings.Default.FilePathFactoryShipment))
      {
        DataFile.Save(_data.CustomerOrders,
          Settings.Default.FilePathCustomerOrders,
          _data.FactoryShipments,
          Settings.Default.FilePathFactoryShipment,
            progressChanged: progressChanged);


      }
      //no destination path provided (=> we don't save data to file system)
      else
      {
        Console.Write("No file/folder paths provided to save generated data.");
      }

      Console.WriteLine();
    }

    /// <summary>
    /// Read csv data from file system.
    /// Either the folder path and the file search patterns must be provided, or 
    /// the paths of a CustomerOrders and a FactoryShipments file. 
    /// Paths may be absolute or relative. Search patterns may contain * char.
    /// </summary>
    private static void LoadData()
    {
      if (_data == null)
      {
        Console.Write($"{nameof(LoadData)} ... ");
        if (!string.IsNullOrEmpty(Settings.Default.FolderPath))
        {
          _data = DataFile.Load(
            Settings.Default.FolderPath,
            Settings.Default.FolderSearchSubs ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly,
            Settings.Default.FolderSearchPatternCustomerOrders,
            Settings.Default.FolderSearchPatternFactoryShipment,
            progressChanged: progressChanged);
        }
        else if (!string.IsNullOrEmpty(Settings.Default.FilePathCustomerOrders) && !string.IsNullOrEmpty(Settings.Default.FilePathFactoryShipment))
        {
          _data = DataFile.Load(
            Settings.Default.FilePathCustomerOrders,
            Settings.Default.FilePathFactoryShipment,
            progressChanged: progressChanged);
        }
        else
        {
          throw new ShipmentReconciliationException("Missing source path.");
        }
        Console.WriteLine(/*"\rLoading data finished."*/);
      }
    }

    /// <summary>
    /// Pre-process customer order and factory shipment records, 
    /// calculating total counts of records and sums of quantites, 
    /// and total surplus and deficit per products.
    /// </summary>
    private static void ValidateData()
    {
      if (_dataWrapper == null)
      {
        Console.Write($"{nameof(ValidateData)} ");
        _dataWrapper = new DataWrapper(_data);

        if (Settings.Default.Verbose)
        {
          if (Settings.Default.DisplayData)
          {
            Console.WriteLine();
            DisplayDataDetailed(_data);
          }
          DisplayDataSummary(_dataWrapper);
        }
        else
        {
          Console.WriteLine($"Surplus: {_dataWrapper.TotalSurplus:N0} items of {_dataWrapper.CountProductSurplus:N0} products, Deficit: {_dataWrapper.TotalDeficit:N0} items of {_dataWrapper.CountProductDeficit:N0} products.");
        }
      }
    }

    /// <summary>
    /// List detailed input data tables on screen
    /// </summary>
    /// <param name="data"></param>
    private static void DisplayDataDetailed(Data data)
    {
      DisplayDetailed(data.CustomerOrders, "CustomerOrders:");
      DisplayDetailed(data.FactoryShipments, "FactoryShipments:");
    }

    /// <summary>
    /// Reconcile the customer orders with the factory shipment
    /// </summary>
    private static void ProcessData()
    {
      Console.Write($"{nameof(ProcessData)} ... ");
      _result = Reconciler.Resolve(_dataWrapper, Settings.Default.OptimizerLimit, progressChanged);
      Console.WriteLine();
    }

    /// <summary>
    /// List detailed result set on screen
    /// </summary>
    private static void DisplayResult()
    {
      if (Settings.Default.Verbose)
      {
        if (Settings.Default.DisplayResult)
        {
          DisplayResultDetailed(_result);
        }
        DisplayResultSummary(_result);
      }
    }

    /// <summary>
    /// Lists reconcoliation result records (orders to fulfill and products to store) on screen
    /// </summary>
    /// <param name="result"></param>
    private static void DisplayResultDetailed(Result result)
    {
      DisplayDetailed(result.CustomerOrdersToFulfill, "Fulfill:");

      DisplayDetailed(result.ProductsToStore, "Store:");
    }

    /// <summary>
    /// Lists records on screen
    /// </summary>
    /// <param name="items"></param>
    /// <param name="title"></param>
    private static void DisplayDetailed(IEnumerable<FactoryShipment> items, string title)
    {
      Console.WriteLine(title);
      Console.WriteLine($"\tItemName\tQuantity");
      foreach (FactoryShipment item in items)
      {
        Console.WriteLine($"\t{item.ItemName}\t{item.Quantity}");
      }
    }

    /// <summary>
    /// Lists records on screen
    /// </summary>
    /// <param name="items"></param>
    /// <param name="title"></param>
    private static void DisplayDetailed(IEnumerable<CustomerOrder> items, string title)
    {
      Console.WriteLine(title);
      Console.WriteLine($"\tID\tCustID\tItemName\tQuantity");
      foreach (CustomerOrder item in items)
      {
        Console.WriteLine($"\t{item.OrderID}\t{item.CustomerID}\t{item.ItemName}\t{item.Quantity}");
      }
    }

    /// <summary>
    /// Save reconciliation result to file system
    /// </summary>
    private static void SaveResult()
    {
      Console.Write($"{nameof(SaveResult)} ... ");

      if (
        //No filename provided where to save records of Customer Orders to fulfill.
        (string.IsNullOrEmpty(Settings.Default.ResultFileNameFulfill))
        ||
        //No file/folder path provided where to save records of Customer Orders to fulfill.
        (!(Path.IsPathRooted(Settings.Default.ResultFileNameFulfill)) && string.IsNullOrEmpty(Settings.Default.ResultFolderPath) && string.IsNullOrEmpty(Settings.Default.FolderPath))
        ||
        //No filename provided where to save records of surplus product quantities to store.
        (string.IsNullOrEmpty(Settings.Default.ResultFileNameStore))
        ||
        //No file/folder path provided where to save records of surplus product quantities to store.
        (!(Path.IsPathRooted(Settings.Default.ResultFileNameStore)) && string.IsNullOrEmpty(Settings.Default.ResultFolderPath) && string.IsNullOrEmpty(Settings.Default.FolderPath))
        )
        { Console.WriteLine("Missing folder path or file name. No results saved."); return; }
      

      string resultFilePathFulfill =
        (System.IO.Path.IsPathRooted(Settings.Default.ResultFileNameFulfill)) ? Settings.Default.ResultFileNameFulfill :
        !string.IsNullOrEmpty(Settings.Default.ResultFolderPath) ? Path.Combine(Settings.Default.ResultFolderPath, Settings.Default.ResultFileNameFulfill) :
         Path.Combine(Settings.Default.FolderPath, Settings.Default.ResultFileNameFulfill);

      string resultFilePathStore =
        (System.IO.Path.IsPathRooted(Settings.Default.ResultFileNameStore)) ? Settings.Default.ResultFileNameStore :
        !string.IsNullOrEmpty(Settings.Default.ResultFolderPath) ? Path.Combine(Settings.Default.ResultFolderPath, Settings.Default.ResultFileNameStore) :
         Path.Combine(Settings.Default.FolderPath, Settings.Default.ResultFileNameStore);

      DataFile.Save(
           _result.CustomerOrdersToFulfill, resultFilePathFulfill,
           _result.ProductsToStore, resultFilePathStore,
           progressChanged: progressChanged);

      Console.WriteLine();
    }

    /// <summary>
    /// List pre-processed data on screen.
    /// </summary>
    /// <param name="dataWrapper"></param>
    /// <param name="title"></param>
    /// <param name="indent"></param>
    private static void DisplayDataSummary(DataWrapper dataWrapper, string title = "Summary:", string indent = "\t")
    {
      Console.WriteLine($"{title}");
      Console.WriteLine($"{indent}CustomerOrder: {dataWrapper.CountItemCustomerOrders:N0} items in {dataWrapper.CountRecordCustomerOrders:N0} orders of {dataWrapper.CountProductCustomerOrders:N0} products.");
      Console.WriteLine($"{indent}FactoryShipment: {dataWrapper.CountItemFactoryShipment:N0} items in {dataWrapper.CountRecordFactoryShipment:N0} shipments of {dataWrapper.CountProductFactoryShipment:N0} products.");
      Console.WriteLine($"{indent}TotalSurplus: {dataWrapper.TotalSurplus:N0} items of {dataWrapper.CountProductSurplus:N0} products.");
      Console.WriteLine($"{indent}TotalDeficit: {dataWrapper.TotalDeficit:N0} items of {dataWrapper.CountProductDeficit:N0} products.");
    }

    /// <summary>
    /// List pre-processed data on screen.
    /// </summary>
    /// <param name="dataWrapper"></param>
    /// <param name="title"></param>
    /// <param name="indent"></param>
    private static void DisplayResultSummary(Result result, string title = "Summary:", string indent = "\t")
    {
      Console.WriteLine($"{title}");
      Console.WriteLine($"{indent}Fulfill: {result.CustomerOrdersToFulfill.Sum(cof => cof.Quantity):N0} items in {result.CustomerOrdersToFulfill.Count():N0} orders.");
      Console.WriteLine($"{indent}Store: {result.ProductsToStore.Sum(pts => pts.Quantity):N0} items of {result.ProductsToStore.Count():N0} products.");
    }

    /// <summary>
    /// Handle application exceptions.
    /// </summary>
    /// <param name="ex"></param>
    private static void HandleWarning(ShipmentReconciliationException ex)
    {
      Console.WriteLine($"Warning: {ex.Message}");
    }

    /// <summary>
    /// Handle unexpected exceptions.
    /// </summary>
    /// <param name="ex"></param>
    private static void HandleError(Exception ex)
    {
      Console.WriteLine($"Error: {ex.Message}");
    }

    /// <summary>
    /// Prompt user to exit application.
    /// </summary>
    private static void PromptFinish()
    {
      if (Settings.Default.AutoExit)
      {
        return;
      }
      Console.WriteLine("Press ENTER to exit...");
      Console.ReadLine();
    }
  }
}
