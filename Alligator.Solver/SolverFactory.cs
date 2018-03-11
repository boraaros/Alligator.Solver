using Alligator.Solver.Algorithm;
using Alligator.Solver.Caches;
using Alligator.Solver.Heuristics;
using System;

namespace Alligator.Solver
{
    public class SolverFactory<TPosition, TPly>
        where TPosition : IPosition<TPly>
    {
        private readonly IExternalLogics<TPosition, TPly> externalLogics;
        private readonly ISolverConfiguration solverConfiguration;
        private readonly Action<string> logger;

        public SolverFactory(IExternalLogics<TPosition, TPly> externalLogics, ISolverConfiguration solverConfiguration, Action<string> logger)
        {
            this.externalLogics = externalLogics ?? throw new ArgumentNullException("externalLogics");
            this.solverConfiguration = solverConfiguration ?? throw new ArgumentNullException("solverConfiguration");
            this.logger = logger ?? throw new ArgumentNullException("logger");
        }

        public SolverFactory(IExternalLogics<TPosition, TPly> externalLogics, ISolverConfiguration solverConfiguration)
            : this(externalLogics, solverConfiguration, (message) => { })
        {
        }

        public ISolver<TPly> Create()
        {
            var solver = new IterativeSearch<TPosition, TPly>(
                externalLogics,
                new CacheTables<TPosition, TPly>(solverConfiguration),
                new HeuristicTables<TPly>(),
                solverConfiguration,
                logger);

            return solver;
        }
    }
}
