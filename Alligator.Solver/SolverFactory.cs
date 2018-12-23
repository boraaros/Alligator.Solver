using Alligator.Solver.Algorithm;
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
        /// Creates new iterative alpha-beta based solver instance.
        /// </summary>
        /// <returns>new solver instance</returns>
        public ISolver<TMove> Create()
        {
            var stm = new SearchTreeManager(solverConfiguration.TimeLimitPerMove);

            var solver = new IterativeDeepeningSearch<TPosition, TMove>(
                rules,
                stm,
                new AlphaBetaPruning<TPosition, TMove>(
                    rules,
                    stm,
                    new HeuristicTables<TMove>(),
                    new CacheTables<TPosition, TMove>(new CachesSettings())),
                logger
                );

            logger($"New solver instance has been created");

            return solver;
        }
    }
}