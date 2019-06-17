namespace ShipmentReconciliation
{
  public class ResultDecision
  {
    public CustomerOrder CustomerOrder { get; private set; }
    public bool Fulfill { get; private set; }
    public ResultDecision(CustomerOrder customerOrder, bool fulfill)
    {
      CustomerOrder = customerOrder;
      Fulfill = fulfill;
    }
  }
}
