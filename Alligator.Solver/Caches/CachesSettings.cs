using System;

namespace Alligator.Solver.Caches
{
    internal class CachesSettings : ICachesSettings
    {
        public int EvaluationTableSizeExponent => 16;
        public int EvaluationTableRetryLimit => 1;
        public int TranspositionTableSizeExponent => 16;
        public int TranspositionTableRetryLimit => 1;
    }
}