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
            if (externalLogics == null)
            {
                throw new ArgumentNullException("externalLogics");
            }
            if (solverConfiguration == null)
            {
                throw new ArgumentNullException("solverConfiguration");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            this.externalLogics = externalLogics;
            this.solverConfiguration = solverConfiguration;
            this.logger = logger;
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
