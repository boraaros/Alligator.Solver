using System;

namespace Alligator.Solver.Algorithm
{
    internal class CachesSettings : ICachesSettings
    {
        public int EvaluationTableSizeExponent => 4;
        public int EvaluationTableRetryLimit => 0;
        public int TranspositionTableSizeExponent => 24;
        public int TranspositionTableRetryLimit => 1;
    }
}