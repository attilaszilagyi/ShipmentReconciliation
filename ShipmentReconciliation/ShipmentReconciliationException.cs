namespace ShipmentReconciliation
{
  /// <summary>
  /// Application exception
  /// </summary>
  public class ShipmentReconciliationException : System.Exception
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
