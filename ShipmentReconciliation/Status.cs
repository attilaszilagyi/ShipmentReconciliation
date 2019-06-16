using System;

namespace ShipmentReconciliation
{

  internal class Status
  {
    public Status(string operation, Action<string> progressChanged, int reportPerCount)
    {
      Operation = operation;
      ProgressChanged = progressChanged;
      ReportPerCount = reportPerCount;
    }
    private int _customerOrderCount;
    public int CustomerOrderCount { get => _customerOrderCount; set { _customerOrderCount = value; Check(value); } }

    private int _factoryShipmentCount;
    public int FactoryShipmentCount { get => _factoryShipmentCount; set { _factoryShipmentCount = value; Check(value); } }

    public string Operation { get; private set; }

    private readonly Action<string> ProgressChanged;

    public int ReportPerCount { get; private set; }

    public string CustomerOrderMessage { get; set; }
    public string FactoryShipmentMessage { get; set; }

    public void Report()
    {
      ProgressChanged?.Invoke($"{Operation} CustomerOrder: {CustomerOrderCount:N0} {CustomerOrderMessage} FactoryShipment: {FactoryShipmentCount:N0} {FactoryShipmentMessage}");
    }

    private void Check(int count)
    {
      if (ReportPerCount>0 && count % ReportPerCount == 0)
      {
        Report();
      }
    }

  }
}

