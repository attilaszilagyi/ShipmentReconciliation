using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ShipmentReconciliation
{
  internal static partial class DataFile
  {
    public static Configuration DefaultConfiguration { get; set; } = new Configuration();

    private static string Trim(string text, int max = 50)
    {
      return (text.Length > max) ? "..." + text.Substring(text.Length - max) : text;
    }

    private static int ProcessRecords<T>(IEnumerable<T> records, System.Action<T> process, System.Action<string> progressChanged, string progressOperation, int progressPerCnt = 100)
    {
      int cntRecord = 0;      
      foreach (T item in records)
      {
        process(item);
        cntRecord++;
        if (cntRecord % progressPerCnt == 0)
        {
          progressChanged($"{progressOperation} {cntRecord:N0}");
        }
      }
      return cntRecord;
    }
  }
}
