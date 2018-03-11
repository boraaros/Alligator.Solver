using Alligator.Solver.Caches;
using Alligator.Solver.Heuristics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alligator.Solver.Algorithm
{
    internal class IterativeSearch<TPosition, TPly> : ISolver<TPly>
        where TPosition : IPosition<TPly>
    {
        private readonly IExternalLogics<TPosition, TPly> externalLogics;
        private readonly IHeuristicTables<TPly> heuristicTables;
        private readonly ICacheTables<TPosition, TPly> cacheTables;
        private readonly ISolverConfiguration solverConfiguration;
        private readonly Action<string> logger;

        private readonly object lockObj = new object();

        public IterativeSearch(
            IExternalLogics<TPosition, TPly> externalLogics,
            ICacheTables<TPosition, TPly> cacheTables,
            IHeuristicTables<TPly> heuristicTables,
            ISolverConfiguration solverConfiguration,
            Action<string> logger)
        {
            this.externalLogics = externalLogics ?? throw new ArgumentNullException(nameof(externalLogics));
            this.cacheTables = cacheTables ?? throw new ArgumentNullException(nameof(cacheTables));
            this.heuristicTables = heuristicTables ?? throw new ArgumentNullException(nameof(heuristicTables));
            this.solverConfiguration = solverConfiguration ?? throw new ArgumentNullException(nameof(solverConfiguration));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            logger("Iterative search has started");

            var isStopRequested = false;
            IMiniMax<TPosition> miniMax = null;
            int solution = 0;
            IList<TPly> forecast = new List<TPly>();
            var task = Task.Factory.StartNew(() =>
            {
                int searchDepthLimit = 0;
                while (!isStopRequested && searchDepthLimit++ <= solverConfiguration.SearchDepthLimit)
                {
                    var settings = new MiniMaxSettings(searchDepthLimit, solverConfiguration.QuiescenceExtensionLimit);
                    miniMax = new NegaScout<TPosition, TPly>(externalLogics, cacheTables, heuristicTables, settings);
                    TPosition position = CreateFromHistory(history);
                    var start = DateTime.Now;
                    int nextSolution;
                    if (searchDepthLimit < solverConfiguration.MinimumSearchDepthToUseMtdf)
                    {
                        nextSolution = SimpleSearch(miniMax, position, -int.MaxValue, int.MaxValue);
                    }
                    else
                    {
                        nextSolution = MtdfSearch(miniMax, position, solution);
                    }
                    if (!isStopRequested)
                    {
                        lock (lockObj)
                        {
                            solution = nextSolution;
                        }
                        forecast = RevealPrincipalVariation(history);

                        logger(string.Format("#{0} | {1} ms | {2} | {3}", 
                            searchDepthLimit, (long)((DateTime.Now - start).TotalMilliseconds), solution, string.Join(" > ", forecast)));
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
            isStopRequested = true;
            miniMax.Stop();
            return solution;
        }

        private int SimpleSearch(IMiniMax<TPosition> miniMax, TPosition position, int alpha, int beta)
        {
            return miniMax.Search(position, alpha, beta);
        }

        private int MtdfSearch(IMiniMax<TPosition> miniMax, TPosition position, int estimatedValue)
        {
            var value = estimatedValue;

            var upperBound = int.MaxValue;
            var lowerBound = -int.MaxValue;

            while (lowerBound < upperBound)
            {
                var beta = Math.Max(value, lowerBound + 1);
                value = SimpleSearch(miniMax, position, beta - 1, beta);
                if (value < beta)
                {
                    upperBound = value;
                }
                else
                {
                    lowerBound = value;
                }   
            }
            return value;
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