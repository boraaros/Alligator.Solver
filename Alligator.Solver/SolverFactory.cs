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

        public SolverFactory(IExternalLogics<TPosition, TPly> externalLogics, ISolverConfiguration solverConfiguration)
        {
            if (externalLogics == null)
            {
                throw new ArgumentNullException("externalLogics");
            }
            if (solverConfiguration == null)
            {
                throw new ArgumentNullException("solverConfiguration");
            }
            this.externalLogics = externalLogics;
            this.solverConfiguration = solverConfiguration;
        }

        public ISolver<TPly> Create()
        {
            var solver = new IterativeSearch<TPosition, TPly>(
                externalLogics,
                new CacheTables<TPosition, TPly>(solverConfiguration),
                new HeuristicTables<TPly>(),
                solverConfiguration);

            return solver;
        }
    }
}
