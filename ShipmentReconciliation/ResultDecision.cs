namespace ShipmentReconciliation
{
  public class ResultDecision
  {
    public CustomerOrder CustomerOrder { get; private set; }
    public bool Fulfill { get; set; }
  }
}
