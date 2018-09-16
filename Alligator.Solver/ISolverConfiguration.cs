using System;

namespace Alligator.Solver
{
    /// <summary>
    /// This component can be used to configure the solver.
    /// </summary>
    public interface ISolverConfiguration
    {
        /// <summary>
        /// The maximum thinking time per move.
        /// </summary>
        TimeSpan TimeLimitPerMove { get; }

        /// <summary>
        /// The maximum number of concurrent tasks enabled by this configuration instance.
        /// </summary>
        int MaxDegreeOfParallelism { get; }        
    }
}