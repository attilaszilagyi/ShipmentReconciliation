using CsvHelper.Configuration;
using System.Collections.Generic;

namespace ShipmentReconciliation
{
  /// <summary>
  /// Csv file operations: Load from file system. Save to file system.
  /// </summary>
  public static partial class DataFile
  {
    /// <summary>
    /// Default Csv options.
    /// </summary>
    public static Configuration DefaultConfiguration { get; set; } = new Configuration();

    private static string Trim(string text, int max = 50)
    {
      return (text.Length > max) ? "..." + text.Substring(text.Length - max) : text;
    }

    private static int ProcessRecords<T>(IEnumerable<T> records, System.Action<T> process, System.Action<string> progressChanged, string operation, int reportPerCount = 100)
    {
      int cntRecord = 0;
      foreach (T item in records)
      {
        process(item);
        cntRecord++;
        if (cntRecord % reportPerCount == 0)
        {
          progressChanged?.Invoke($"{operation} {cntRecord:N0}");
        }
      }
      return cntRecord;
    }

    private static int ProcessRecords<T>(IEnumerable<T> records, System.Action<T> process, System.Action<int, string> progressChanged, string operation)
    {
      int cntRecord = 0;
      foreach (T item in records)
      {
        cntRecord++;
        progressChanged?.Invoke(cntRecord, operation);
        process(item);
      }
      return cntRecord;
    }
  }
}
