using System;

namespace Alligator.Solver
{
    public interface ISolverConfiguration
    {
        int EvaluationTableSizeExponent { get; }
        int EvaluationTableRetryLimit { get; }
        int TranspositionTableSizeExponent { get; }
        int TranspositionTableRetryLimit { get; }
    }
}
