using Alligator.Solver.Caches;
using Alligator.Solver.Heuristics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alligator.Solver.Algorithm
{
    class IterativeSearch<TPosition, TPly> : ISolver<TPly>
        where TPosition : IPosition<TPly>
    {
        private readonly IExternalLogics<TPosition, TPly> externalLogics;
        private readonly IHeuristicTables<TPly> heuristicTables;
        private readonly ICacheTables<TPosition, TPly> cacheTables;
        private readonly ISolverConfiguration solverConfiguration;

        private readonly object lockObj = new object();

        public IterativeSearch(
            IExternalLogics<TPosition, TPly> externalLogics,
            ICacheTables<TPosition, TPly> cacheTables,
            IHeuristicTables<TPly> heuristicTables,
            ISolverConfiguration solverConfiguration)
        {
            if (externalLogics == null)
            {
                throw new ArgumentNullException("externalLogics");
            }
            if (cacheTables == null)
            {
                throw new ArgumentNullException("cacheTables");
            }
            if (heuristicTables == null)
            {
                throw new ArgumentNullException("heuristicTables");
            }
            if (solverConfiguration == null)
            {
                throw new ArgumentNullException("solverConfiguration");
            }
            this.externalLogics = externalLogics;
            this.cacheTables = cacheTables;
            this.heuristicTables = heuristicTables;
            this.solverConfiguration = solverConfiguration;
        }

        public int Maximize(IList<TPly> history, out IList<TPly> forecast)
        {
            if (history == null)
            {
                throw new ArgumentNullException("history");
            }

            TPosition position = CreateFromHistory(history);
            if (position.IsEnded)
            {
                throw new InvalidOperationException("The game is already over");
            }

            return Run(history, out forecast);
        }

        private int Run(IList<TPly> history, out IList<TPly> principalVariation)
        {
            bool stop = false;
            IMiniMax<TPosition> miniMax = null;
            int solution = 0;
            IList<TPly> forecast = new List<TPly>();
            var task = Task.Factory.StartNew(() =>
            {
                int searchDepthLimit = 0;
                while (!stop && searchDepthLimit++ <= solverConfiguration.SearchDepthLimit)
                {
                    var settings = new MiniMaxSettings(searchDepthLimit, solverConfiguration.QuiescenceExtensionLimit);
                    miniMax = new NegaScout<TPosition, TPly>(externalLogics, cacheTables, heuristicTables, settings);
                    TPosition position = CreateFromHistory(history);
                    var nextSolution = miniMax.Search(position);
                    if (!stop)
                    {
                        lock (lockObj)
                        {
                            solution = nextSolution;
                        }
                        forecast = RevealPrincipalVariation(history);
                    }
                    var p = CreateFromHistory(history);
                    foreach (var ply in forecast)
                    {
                        p.Do(ply);
                    }
                    if (p.IsEnded)
                    {
                        break;
                    }
                }
            });

            task.Wait(solverConfiguration.TimeLimitPerMove);
            principalVariation = forecast;
            stop = true;
            miniMax.Stop();
            return solution;
        }

        private TPosition CreateFromHistory(IList<TPly> history)
        {
            TPosition position = externalLogics.CreateEmptyPosition();
            foreach (var ply in history)
            {
                position.Do(ply);
            }
            return position;
        }

        private IList<TPly> RevealPrincipalVariation(IList<TPly> history)
        {
            IList<TPly> principalVariation = new List<TPly>();
            TPosition current = CreateFromHistory(history);
            Transposition<TPly> transposition;
            while (cacheTables.TryGetTransposition(current, out transposition))
            {
                principalVariation.Add(transposition.BestStrategy);
                current.Do(transposition.BestStrategy);
            }
            return principalVariation;
        }
    }
}