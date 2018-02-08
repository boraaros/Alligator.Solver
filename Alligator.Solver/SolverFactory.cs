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
            throw new NotImplementedException();
        }
    }
}
