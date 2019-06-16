namespace ShipmentReconciliation
{
  /// <summary>
  /// Application exception
  /// </summary>
  internal class ShipmentReconciliationException : System.Exception
  {
    public ShipmentReconciliationException():base()
    {

    }
    public ShipmentReconciliationException(string message):base(message)
    {

    }
    public ShipmentReconciliationException(string message, System.Exception innerException):base(message, innerException)
    {

    }
  }
}
