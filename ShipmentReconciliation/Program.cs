﻿using ShipmentReconciliation.Properties;
using System;
using System.Configuration;
using System.Linq;

namespace ShipmentReconciliation
{
  internal class Program
  {
    private static Data _data;
    private static DataWrapper _dataWrapper;
    private static CsvHelper.Configuration.Configuration _csvConfiguration;

    private static readonly System.Action<string> progressChanged = text => { Console.Write("\r" + text.PadRight(100)); };

    private static void Main(string[] args)
    {

      DisplayTitle();
      try
      {
        LoadSettings(args);
        DisplaySettings();
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


    private static void DisplayTitle()
    {
      Console.WriteLine(System.Reflection.Assembly.GetEntryAssembly().FullName);
    }

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

      _csvConfiguration = !string.IsNullOrEmpty(Settings.Default.CsvConfigurationCulture) ? new CsvHelper.Configuration.Configuration(new System.Globalization.CultureInfo(Settings.Default.CsvConfigurationCulture)) : new CsvHelper.Configuration.Configuration();
      if (!string.IsNullOrEmpty(Settings.Default.CsvConfigurationDelimiter))
      {
        _csvConfiguration.Delimiter = Settings.Default.CsvConfigurationDelimiter;
      }

      if (!string.IsNullOrEmpty(Settings.Default.CsvConfigurationEncoding))
      {
        _csvConfiguration.Encoding = System.Text.Encoding.GetEncoding(Settings.Default.CsvConfigurationEncoding);
      }
    }

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

    private static bool PromptStart()
    {
      if (Settings.Default.GenerateData || Settings.Default.ProcessData)
      {
          string operation = string.Empty;
          if (Settings.Default.GenerateData)
            operation += $"{nameof(GenerateData)} ";
          if (Settings.Default.ProcessData)
            operation += $"{nameof(ProcessData)} ";
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

    private static void SaveData()
    {
      Console.Write($"{nameof(SaveData)} ... ");

      if (!string.IsNullOrEmpty(Settings.Default.FolderPath))
      {
        var subFolderID = DateTime.Now.Ticks.ToString();
        string folderPath = System.IO.Path.Combine(Settings.Default.FolderPath, subFolderID);
        Console.Write(folderPath);
        DataFile.Save(_data, folderPath,
            customerOrdersCsvConfiguration: _csvConfiguration,
            factoryShipmentsCsvConfiguration: _csvConfiguration,
            progressChanged: progressChanged);
        
      }
      else if (!string.IsNullOrEmpty(Settings.Default.FilePathCustomerOrders) && !string.IsNullOrEmpty(Settings.Default.FilePathFactoryShipment))
      {
        DataFile.Save(_data.CustomerOrders, 
          Settings.Default.FilePathCustomerOrders,
          _data.FactoryShipments,
          Settings.Default.FilePathFactoryShipment,
            customerOrdersCsvConfiguration: _csvConfiguration,
            factoryShipmentsCsvConfiguration: _csvConfiguration,
            progressChanged: progressChanged);
        
        
      }
      else
      { Console.Write("No saving path provided."); }

        Console.WriteLine();
    }

    private static void ProcessData()
    {

    }

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
            customerOrdersCsvConfiguration: _csvConfiguration,
            factoryShipmentsCsvConfiguration: _csvConfiguration,
            progressChanged: progressChanged);
        }
        else if (!string.IsNullOrEmpty(Settings.Default.FilePathCustomerOrders) && !string.IsNullOrEmpty(Settings.Default.FilePathFactoryShipment))
        {
          _data = DataFile.Load(
            Settings.Default.FilePathCustomerOrders,
            Settings.Default.FilePathFactoryShipment,
            customerOrdersCsvConfiguration: _csvConfiguration,
            factoryShipmentsCsvConfiguration: _csvConfiguration,
            progressChanged: progressChanged);
        }
        else
        {
          throw new ShipmentReconciliationException("Missing source path.");
        }
        Console.WriteLine(/*"\rLoading data finished."*/);
      }
    }

    private static void ValidateData()
    {
      if (_dataWrapper == null)
      {
        Console.Write($"{nameof(ValidateData)} ");
        _dataWrapper = new DataWrapper(_data);
        
        if (Settings.Default.Verbose)
        {
          DisplaySummary(_dataWrapper);
        }
        else
        {
          Console.WriteLine($"Surplus: {_dataWrapper.TotalSurplus:N0} items of {_dataWrapper.CountProductSurplus:N0} products, Deficit: {_dataWrapper.TotalDeficit:N0} items of {_dataWrapper.CountProductDeficit:N0} products.");
        }
      }
    }

    private static void DisplaySummary(DataWrapper dataWrapper, string title = "Summary:", string indent = "\t")
    {
      Console.WriteLine($"{title}");
      Console.WriteLine($"{indent}CustomerOrder: {dataWrapper.CountItemCustomerOrders:N0} items in {dataWrapper.CountRecordCustomerOrders:N0} orders of {dataWrapper.CountProductCustomerOrders:N0} products.");
      Console.WriteLine($"{indent}FactoryShipment: {dataWrapper.CountItemFactoryShipment:N0} items in {dataWrapper.CountRecordFactoryShipment:N0} shipments of {dataWrapper.CountProductFactoryShipment:N0} products.");
      Console.WriteLine($"{indent}TotalSurplus: {dataWrapper.TotalSurplus:N0} items of {dataWrapper.CountProductSurplus:N0} products.");
      Console.WriteLine($"{indent}TotalDeficit: {dataWrapper.TotalDeficit:N0} items of {dataWrapper.CountProductDeficit:N0} products.");

    }

    private static void HandleWarning(ShipmentReconciliationException ex)
    {
      Console.WriteLine($"Warning: {ex.Message}");
    }

    private static void HandleError(Exception ex)
    {
      Console.WriteLine($"Error: {ex.Message}");
    }

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
