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

            logger("Search algorithm has been started");
            searchTreeManager.Restart();

            TPosition position = CreatePosition(moveHistory);

            var candidatesPerIteration = new List<ICollection<TMove>>();

            while (true)
            {
                var nextCandidates = BestNodeSearch(position);

                candidatesPerIteration.Add(nextCandidates);
                searchTreeManager.IterationCompleted();
                logger($"Iteration #{candidatesPerIteration.Count} completed with result: {string.Join(", ", nextCandidates)}");

                if (searchTreeManager.IsStopRequested())
                {
                    logger("Search algorithm has been finished because the time is over");
                    break;
                }
                if (IsStable(candidatesPerIteration))
                {
                    logger("Search algorithm has been finished because the result is stable");
                    break;
                }
            }

            var optimalNextMove = SelectMove(candidatesPerIteration);
            logger($"Optimal next move: {optimalNextMove}");
            return optimalNextMove;
        }

        private ICollection<TMove> BestNodeSearch(TPosition position) // TODO: initial guess from previous iteration!
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
                    int value = NullWindowTest(position, move, guess);

                    if (searchTreeManager.IsStopRequested())
                    {
                        break;
                    }
                    if (value >= guess)
                    {
                        newCandidates.Add(move);
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
            }
        
            return candidates;
        }

        private TPosition CreatePosition(IEnumerable<TMove> moveHistory)
        {
            var position = rules.InitialPosition();
            foreach (var move in moveHistory)
            {
                position.Take(move);
            }

            if (!rules.LegalMovesAt(position).Any())
            {
                throw new InvalidOperationException("Next move calculation failed because the game is already over");
            }

            return position;
        }

        private bool IsStable(IEnumerable<ICollection<TMove>> candidatesPerIteration)
        {
            return candidatesPerIteration.Any()
                && candidatesPerIteration.Last().Any(t => GetStabilityIndex(candidatesPerIteration, t) >= 10); // TODO: magic number
        }

        private TMove SelectMove(IEnumerable<ICollection<TMove>> candidatesPerIteration)
        {
            if (!candidatesPerIteration.Any())
            {
                throw new InvalidOperationException($"Solver does not have enough time to optimize next move");
            }

            return candidatesPerIteration.Last().OrderByDescending(t => GetStabilityIndex(candidatesPerIteration, t)).First();
        }

        private int GetStabilityIndex(IEnumerable<ICollection<TMove>> candidatesPerIteration, TMove move)
        {
            return candidatesPerIteration.Reverse().TakeWhile(t => t.Contains(move)).Count();
        }

        private int NullWindowTest(TPosition position, TMove move, int guess)
        {
            position.Take(move);
            var value = -minimax.Search(position, -guess, -(guess - 1));
            position.TakeBack();
            return value;
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

            var guess = (int)(alpha + (count - 1.0) / count * (beta - alpha));

            if (guess == alpha)
            {
                return guess + 1;
            }
            else if (guess == beta)
            {
                return guess - 1;
            }
            return guess;
        }
    }
}