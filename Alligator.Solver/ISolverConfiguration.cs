using System;

namespace Alligator.Solver
{
    public interface ISolverConfiguration
    {
        TimeSpan TimeLimitPerMove { get; }
        int SearchDepthLimit { get; }
        int QuiescenceExtensionLimit { get; }
        int EvaluationTableSizeExponent { get; }
        int EvaluationTableRetryLimit { get; }
        int TranspositionTableSizeExponent { get; }
        int TranspositionTableRetryLimit { get; }
        int MinimumSearchDepthToUseMtdf { get; }
    }
}
