using System;

namespace ShipmentReconciliation
{
  /// <summary>
  /// Helper class for progress report of multi-threaded tasks of processing  
  /// Customer Order records and Factory Shipment records in parallel.
  /// </summary>
  internal class ProgressStatus
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="operation">Title text for progress report</param>
    /// <param name="progressChanged">Callback for progress report</param>
    /// <param name="reportPerCount">Aggregate this number of results before calling back</param>
    public ProgressStatus(string operation, Action<string> progressChanged, int reportPerCount)
    {
      Operation = operation;
      ProgressChanged = progressChanged;
      ReportPerCount = reportPerCount;
    }

    /// <summary>
    /// Number of processed Customer Order records
    /// </summary>
    public int CustomerOrderCount { get => _customerOrderCount; set { _customerOrderCount = value; Check(value); } }
    private int _customerOrderCount;

    /// <summary>
    /// Number of processed Factory Shipment records
    /// </summary>
    public int FactoryShipmentCount { get => _factoryShipmentCount; set { _factoryShipmentCount = value; Check(value); } }
    private int _factoryShipmentCount;

    /// <summary>
    /// Title text for progress report
    /// </summary>
    public string Operation { get; private set; }

    /// <summary>
    /// Callback for progress report
    /// </summary>
    private readonly Action<string> ProgressChanged;

    /// <summary>
    /// Aggregate this number of records before reporting progress
    /// </summary>
    public int ReportPerCount { get; private set; }

    /// <summary>
    /// Status text of the processing of Customer Order records.
    /// </summary>
    public string CustomerOrderMessage { get; set; }

    /// <summary>
    /// Status text of the processing of Factory Shipment records.
    /// </summary>
    public string FactoryShipmentMessage { get; set; }

    /// <summary>
    /// Invokes progress report callback with the actual status values.
    /// </summary>
    public void Report()
    {
      ProgressChanged?.Invoke($"{Operation} CustomerOrder: {CustomerOrderCount:N0} {CustomerOrderMessage} FactoryShipment: {FactoryShipmentCount:N0} {FactoryShipmentMessage}");
    }

    /// <summary>
    /// Automatically invokes progress report callback with the actual status values 
    /// in case the processed record count is a multiplication of ReportPerCount value.
    /// </summary>
    /// <param name="count"></param>
    private void Check(int count)
    {
      if (ReportPerCount>0 && count % ReportPerCount == 0)
      {
        Report();
      }
    }

  }
}

