using Microsoft.SolverFoundation.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShipmentReconciliation
{
  /// <summary>
  /// Optimization with 01-Knapsack algorithm
  /// </summary>
  public static class ResolverComplex
  {
    /// <summary>
    /// Optimize with 01-Knapsack algorithm
    /// </summary>
    /// <param name="optimizerLimit">Maximum number of different combinations to try. Zero: no limit.</param>
    /// <param name="shipped">Total quantity of Factory Shipments of the product</param>
    /// <param name="orders">List of Customer Orders of the product</param>
    /// <param name="efficiencyMin">Minimal efficiency of a combination. Restriction of the 01-Knapsack solver.</param>
    /// <param name="efficiency">Efficiency achieved by the 01-Knapsack solver. This is a return parameter. If it is 0, then no results provided back.</param>
    /// <returns></returns>
    public static IEnumerable<ResultDecision> Resolve(int optimizerLimit, int shipped, CustomerOrder[] orders, double efficiencyMin, out double efficiency)
    {
      efficiency = 0;
      int numItems = orders.Length;
      if (numItems == 0)
      {
        return null;
      }
      List<int[]> patterns = null;
      int[] quantities = orders.Select(o => o.Quantity).ToArray();
      try
      {
        Solve01Knapsack(numItems, optimizerLimit, shipped, quantities, efficiencyMin, out patterns);
      }
      catch (Exception)
      {
        return null;
      }
      int sSum = 0;
      int[] sPattern = null;
      foreach (int[] pattern in patterns)
      {
        int sum = Sum(numItems, quantities, pattern);
        if (sum > sSum) { sSum = sum; sPattern = pattern; }
      }
      if (sPattern == null)
      {
        return null;
      }
      HashSet<ResultDecision> resultDecisions = new HashSet<ResultDecision>();
      for (int i = 0; i < numItems; i++)
      {
        resultDecisions.Add(new ResultDecision(orders[i], sPattern[i] == 1));
      }
      efficiency = sSum / (double)shipped;
      return resultDecisions;
    }

    private static int Sum(int numItems, int[] quantities, int[] pattern)
    {
      int total = 0;
      for (int i = 0; i < numItems; i++)
      {
        total += pattern[i] * quantities[i];
      }
      return total;
    }

    /// <summary>
    /// Knapsack enumerator -- enumerate up to "numAnswers" combinations of "weights" such that the sum of the weights is less than the weight limit.
    /// It places the patterns of items inside the list of patterns.  The efficiency parameter ensures that we don't output any which use less than "efficiency" percent
    /// off the weightlimit.
    /// </summary>
    /// <param name="maxAnswers">maximum number of combinations to get out.  Limits runtime.  If zero return all.</param>
    /// <param name="weights">weight of each item to go into the knapsack</param>
    /// <param name="weightLimit">knapsack weight limit</param>
    /// <param name="efficiency">limit patterns to use at least this % of the weight limit (between 0.0 and 1.0) </param>
    /// <param name="patterns">output list of patterns of inclusion of the weights.</param>
    public static void Solve01Knapsack(int numItems, int maxAnswers, int weightLimit, int[] weights, double efficiency, out List<int[]> patterns)
    {

      ConstraintSystem solver = ConstraintSystem.CreateSolver();
      //CspDomain dom = solver.CreateIntegerInterval(0, weightLimit);
      CspDomain dom = solver.CreateIntegerInterval(0, 1);

      CspTerm knapsackSize = solver.Constant(weightLimit);

      // these represent the quantity of each item.
      CspTerm[] itemQty = solver.CreateVariableVector(dom, "Quantity", numItems);
      CspTerm[] itemWeights = new CspTerm[numItems];

      for (int cnt = 0; cnt < numItems; cnt++)
      {
        itemWeights[cnt] = solver.Constant(weights[cnt]);
      }

      // contributors to the weight (weight * variable value)
      CspTerm[] contributors = new CspTerm[numItems];
      for (int cnt = 0; cnt < numItems; cnt++)
      {
        contributors[cnt] = itemWeights[cnt] * itemQty[cnt];

      }

      // single constraint
      CspTerm knapSackCapacity = solver.GreaterEqual(knapsackSize, solver.Sum(contributors));
      solver.AddConstraints(knapSackCapacity);

      // must be efficient
      CspTerm knapSackAtLeast = solver.LessEqual(knapsackSize * efficiency, solver.Sum(contributors));
      solver.AddConstraints(knapSackAtLeast);

      // start counter and allocate a list for the results.
      int nanswers = 0;
      patterns = new List<int[]>();

      ConstraintSolverSolution sol = solver.Solve();
      while (sol.HasFoundSolution)
      {
        int[] pattern = new int[numItems];
        // extract this pattern from the enumeration.
        for (int cnt = 0; cnt < numItems; cnt++)
        {
          sol.TryGetValue(itemQty[cnt], out object val);
          pattern[cnt] = (int)val;
        }
        // add it to the output.
        patterns.Add(pattern);
        nanswers++;
        // stop if we reach the limit of results.
        if (maxAnswers > 0 && nanswers >= maxAnswers)
        {
          break;
        }

        sol.GetNext();
      }
    }


  }
}
