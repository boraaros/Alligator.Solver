using Alligator.Solver.Caches;
using Alligator.Solver.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alligator.Solver.Algorithm
{
    internal class IterativeSearch<TPosition, TMove> : ISolver<TMove>
        where TPosition : IPosition<TMove>
    {
        private readonly IRules<TPosition, TMove> rules;
        private readonly IHeuristicTables<TMove> heuristicTables;
        private readonly ICacheTables<TPosition, TMove> cacheTables;
        private readonly ISolverConfiguration solverConfiguration;
        private readonly IAlgorithmSettings algorithmSettings;
        private readonly Action<string> logger;

        private readonly object lockObj = new object();

        public IterativeSearch(
            IRules<TPosition, TMove> rules,
            ICacheTables<TPosition, TMove> cacheTables,
            IHeuristicTables<TMove> heuristicTables,
            ISolverConfiguration solverConfiguration,
            IAlgorithmSettings algorithmSettings,
            Action<string> logger)
        {
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            this.cacheTables = cacheTables ?? throw new ArgumentNullException(nameof(cacheTables));
            this.heuristicTables = heuristicTables ?? throw new ArgumentNullException(nameof(heuristicTables));
            this.solverConfiguration = solverConfiguration ?? throw new ArgumentNullException(nameof(solverConfiguration));
            this.algorithmSettings = algorithmSettings ?? throw new ArgumentNullException(nameof(algorithmSettings));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public TMove CalculateNextMove(IList<TMove> history)
        {
            if (history == null) throw new ArgumentNullException(nameof(history));

            TPosition position = CreateFromHistory(history);

            if (!rules.LegalMovesAt(position).Any())
            {
                throw new InvalidOperationException("Next move calculation failed, because the game is already over");
            }

            return Run(history);
        }

        private TMove Run(IList<TMove> history)
        {
            logger("Iterative search has been started");

            var isStopRequested = false;
            IMiniMax<TPosition> miniMax = null;
            int solution = 0;
            IList<TMove> forecast = new List<TMove>();

            var task = Task.Factory.StartNew(() =>
            {
                int searchDepthLimit = 0;
                while (!isStopRequested && !IsFullSearch(history, forecast) && searchDepthLimit++ <= algorithmSettings.SearchDepthLimit)
                {
                    var settings = new MiniMaxSettings(searchDepthLimit, algorithmSettings.QuiescenceExtensionLimit);
                    miniMax = new NegaScout<TPosition, TMove>(rules, cacheTables, heuristicTables, settings); // TODO: ioc
                    TPosition position = CreateFromHistory(history);
                    var start = DateTime.Now;
                    int nextSolution;
                    if (searchDepthLimit < algorithmSettings.MinimumSearchDepthToUseMtdf)
                    {
                        nextSolution = SimpleSearch(miniMax, position, -int.MaxValue, int.MaxValue);
                    }
                    else
                    {
                        nextSolution = MtdfSearch(miniMax, position, solution);
                    }
                    if (!isStopRequested)
                    {
                        solution = nextSolution;
                        forecast = RevealPrincipalVariation(history);

                        logger(string.Format("#{0} | {1} ms | {2} | {3}", 
                            searchDepthLimit, (long)((DateTime.Now - start).TotalMilliseconds), solution, string.Join(" > ", forecast)));
                    }
                }
            });

            task.Wait(solverConfiguration.TimeLimitPerMove);
            isStopRequested = true;
            miniMax.Stop();
            return forecast[0];
        }

        private bool IsFullSearch(IList<TMove> history, IList<TMove> forecast)
        {
            var position = CreateFromHistory(history);
            foreach (var move in forecast)
            {
                position.Take(move);
            }
            return !rules.LegalMovesAt(position).Any();
        }

        private int SimpleSearch(IMiniMax<TPosition> miniMax, TPosition position, int alpha, int beta)
        {
            return miniMax.Start(position, alpha, beta);
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

        private TPosition CreateFromHistory(IList<TMove> history)
        {
            TPosition position = rules.InitialPosition();
            foreach (var move in history)
            {
                position.Take(move);
            }
            return position;
        }

        private IList<TMove> RevealPrincipalVariation(IList<TMove> history)
        {
            IList<TMove> principalVariation = new List<TMove>();
            TPosition current = CreateFromHistory(history);
            Transposition<TMove> transposition;
            while (cacheTables.TryGetTransposition(current, out transposition))
            {
                principalVariation.Add(transposition.BestStrategy);
                current.Take(transposition.BestStrategy);
            }
            return principalVariation;
        }
    }
}