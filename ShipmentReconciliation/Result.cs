using System.Collections.Generic;

namespace ShipmentReconciliation
{
  public class Result
  {
    public HashSet<ResultDecision> Decisions { get; set; } = new HashSet<ResultDecision>();
  }
}
