using System;

namespace Alligator.Solver.Demo
{
    internal class SolverConfiguration : ISolverConfiguration
    {
        public TimeSpan TimeLimitPerMove => TimeSpan.FromSeconds(1);

        public int MaxDegreeOfParallelism => 1;
    }
}