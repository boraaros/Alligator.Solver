using Alligator.Solver.Algorithm;
using Alligator.Solver.Caches;
using Alligator.Solver.Heuristics;
using System;

namespace Alligator.Solver
{
    /// <summary>
    /// Provides the solver instances.
    /// </summary>
    /// <typeparam name="TPosition">type of positions in the specified game</typeparam>
    /// <typeparam name="TMove">type of moves in the specified game</typeparam>
    public class SolverFactory<TPosition, TMove>
        where TPosition : IPosition<TMove>
    {
        private readonly IRules<TPosition, TMove> rules;
        private readonly ISolverConfiguration solverConfiguration;
        private readonly Action<string> logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SolverFactory(IRules<TPosition, TMove> rules, ISolverConfiguration solverConfiguration, Action<string> logger)
        {
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            this.solverConfiguration = solverConfiguration ?? throw new ArgumentNullException(nameof(solverConfiguration));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SolverFactory(IRules<TPosition, TMove> rules, ISolverConfiguration solverConfiguration)
            : this(rules, solverConfiguration, (logMessage) => { })
        {
        }

        /// <summary>
        /// Creates new iterative alpha-beta solver instance.
        /// </summary>
        /// <returns>new solver instance</returns>
        public ISolver<TMove> Create()
        {
            return new IterativeSearch<TPosition, TMove>(
                rules,
                new CacheTables<TPosition, TMove>(new CachesSettings()),
                new HeuristicTables<TMove>(),
                solverConfiguration,
                new AlgorithmSettings(),
                logger);
        }
    }
}