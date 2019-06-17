using System.Collections.Generic;

namespace ShipmentReconciliation
{
  public static class Batch
  {
    public static int ProcessRecords<T>(IEnumerable<T> records, System.Action<T> process, System.Action<string> progressChanged, string operation, int reportPerCount = 100)
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

    public static int ProcessRecords<T>(IEnumerable<T> records, System.Action<T> process, System.Action<int, string> progressChanged, string operation)
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
