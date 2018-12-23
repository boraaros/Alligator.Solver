using System;

namespace Alligator.Solver.Algorithm
{
    internal interface ICachesSettings
    {
        int EvaluationTableSizeExponent { get; }
        int EvaluationTableRetryLimit { get; }
        int TranspositionTableSizeExponent { get; }
        int TranspositionTableRetryLimit { get; }
    }
}