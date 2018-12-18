using System;
using System.Collections.Generic;
using System.Linq;

namespace Alligator.Solver.Algorithm
{
    internal class IterativeDeepeningSearch<TPosition, TMove> : ISolver<TMove>
        where TPosition : IPosition<TMove>
    {
        private readonly IRules<TPosition, TMove> rules;
        private readonly ISearchTreeManager searchTreeManager;
        private readonly IAlphaBetaPruning<TPosition> minimax;
        private readonly Action<string> logger;

        public IterativeDeepeningSearch(
            IRules<TPosition, TMove> rules,
            ISearchTreeManager searchTreeManager,
            IAlphaBetaPruning<TPosition> minimax,
            Action<string> logger)
        {
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            this.searchTreeManager = searchTreeManager ?? throw new ArgumentNullException(nameof(searchTreeManager));
            this.minimax = minimax ?? throw new ArgumentNullException(nameof(minimax));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public TMove OptimizeNextMove(IList<TMove> moveHistory)
        {
            if (moveHistory == null) throw new ArgumentNullException(nameof(moveHistory));

            searchTreeManager.Restart();

            var position = rules.InitialPosition();
            foreach (var move in moveHistory)
            {
                position.Take(move);
            }

            if (!rules.LegalMovesAt(position).Any())
            {
                throw new InvalidOperationException("Next move calculation failed, because the game is already over");
            }

            TMove result = default(TMove);

            int iterationCount = 0;
            int stabilityPoint = 0;

            while (true)
            {
                var nextCandidate = BestNodeSearch(position);

                if (searchTreeManager.IsStopRequested())
                {
                    break;
                }

                if (nextCandidate.Equals(result))
                {
                    stabilityPoint++;
                }
                else
                {
                    stabilityPoint = 1;
                }

                result = nextCandidate;
                searchTreeManager.IterationCompleted();
                iterationCount++;
                logger($"Iteration #{iterationCount} has been completed with result: {result}"); 
                
                if (stabilityPoint == 10)
                {
                    break;
                }
            }

            if (iterationCount == 0)
            {
                throw new InvalidOperationException($"Solver was not enough time to optimize next move");
            }

            return result;
        }

        private TMove BestNodeSearch(TPosition position) // TODO: initial guess from previous iteration!
        {
            var alpha = -int.MaxValue;
            var beta = int.MaxValue;

            IList<TMove> candidates = rules.LegalMovesAt(position).ToList();

            while (alpha + 1 < beta && candidates.Count > 1)
            {
                int guess = NextGuess(alpha, beta, candidates.Count);

                var newCandidates = new List<TMove>();

                foreach (var move in candidates)
                {
                    position.Take(move);
                    var value = -minimax.Search(position, -guess, -(guess - 1));

                    if (value >= guess)
                    {
                        newCandidates.Add(move);
                    }
                    position.TakeBack();

                    if (searchTreeManager.IsStopRequested())
                    {
                        break;
                    }
                }

                if (newCandidates.Count > 0)
                {
                    candidates = newCandidates;
                    alpha = guess;
                }
                else
                {
                    beta = guess;
                }

                if (searchTreeManager.IsStopRequested())
                {
                    break;
                }
            }
        
            return candidates.First();
        }

        private int NextGuess(int alpha, int beta, int count)
        {
            if (alpha <= 0)
            {
                beta = Math.Min(beta, int.MaxValue / 2);
            }

            if (beta >= 0)
            {
                alpha = Math.Max(alpha, -int.MaxValue / 2);
            }

            return (int)(alpha + (count - 1.0) / count * (beta - alpha));
        }
    }
}