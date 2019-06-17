using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentReconciliation
{
  /// <summary>
  /// Csv file operations: Read from file system. Write to file system.
  /// </summary>
  public static class CsvFile
  {
    /// <summary>
   /// Default Csv options.
   /// </summary>
    public static Configuration DefaultConfiguration { get; set; } = new Configuration();

    /// <summary>
    /// Writes records to csv file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="records">Items to write to file</param>
    /// <param name="filePath">Absolute or relative file path</param>
    /// <param name="csvConfiguration">Csv configuration. If it is null, then the default options will be used.</param>
    /// <param name="progressChanged">Callback to report progress</param>
    /// <param name="operation">Progress report title text</param>
    public static void WriteToFile<T>(IEnumerable<T> records, string filePath, Configuration csvConfiguration, System.Action<int, string> progressChanged, string operation)
    {
      string progressTitle = $"{operation} {nameof(WriteToFile)} {filePath.TrimPath()}";
      using (StreamWriter writer = new StreamWriter(filePath))
      using (CsvWriter csv = new CsvWriter(writer, csvConfiguration ?? DefaultConfiguration))
      {
        if (progressChanged == null)
        { csv.WriteRecords(records); }
        else
        {
          csv.WriteHeader<T>();
          int cntRecords = Batch.ProcessRecords(records, (item) => { csv.NextRecord(); csv.WriteRecord(item); }, progressChanged, null);
          
        }

      }
    }
    /// <summary>
    /// Reads records from csv file.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath">Absolute or relative file path</param>
    /// <param name="csvConfiguration">Csv configuration. If it is null, then the default options will be used.</param>
    /// <param name="progressChanged">Callback to report progress</param>
    /// <param name="operation">Progress report title text</param>
    /// <param name="cntRecords">Number of read records</param>
    /// <returns></returns>
    public static IList<T> ReadFromFile<T>(string filePath, Configuration csvConfiguration, System.Action<string> progressChanged, string operation, out int cntRecords)
    {
      string progressTitle = $"{operation} {nameof(ReadFromFile)} {filePath.TrimPath()}";
      progressChanged?.Invoke(progressTitle);
      List<T> cache = new List<T>();
      using (StreamReader reader = new StreamReader(filePath))
      using (CsvHelper.CsvReader csv = new CsvHelper.CsvReader(reader, csvConfiguration ?? CsvFile.DefaultConfiguration))
      {
        csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.Replace(" ", "");
        IEnumerable<T> records = csv.GetRecords<T>();
        if (progressChanged == null)
        { cache.AddRange(records); cntRecords = cache.Count; }
        else
        {
          cntRecords = Batch.ProcessRecords(records, (item) => { cache.Add(item); }, progressChanged, progressTitle);
          progressChanged($"{progressTitle} {cntRecords:N0}");
        }
      }
      return cache;
    }

  }
}
