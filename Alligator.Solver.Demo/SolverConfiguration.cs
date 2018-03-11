using System;

namespace Alligator.Solver.Demo
{
    internal class SolverConfiguration : ISolverConfiguration
    {
        public TimeSpan TimeLimitPerMove
        {
            get { return TimeSpan.FromSeconds(1); }
        }

        public int SearchDepthLimit
        {
            get { return 10; }
        }

        public int QuiescenceExtensionLimit
        {
            get { return 0; }
        }

        public int EvaluationTableSizeExponent
        {
            get { return 4; }
        }

        public int EvaluationTableRetryLimit
        {
            get { return 0; }
        }

        public int TranspositionTableSizeExponent
        {
            get { return 20; }
        }

        public int TranspositionTableRetryLimit
        {
            get { return 0; }
        }

        public int MinimumSearchDepthToUseMtdf
        {
            get { return 4; }
        }
    }
}
