using System;
using System.Linq;
using System.Collections.Generic;

namespace Alligator.Solver.Algorithm
{
    internal class AlphaBetaPruning<TPosition, TMove> : IAlphaBetaPruning<TPosition>
        where TPosition : IPosition<TMove>
    {
        private readonly IRules<TPosition, TMove> rules;
        private readonly ISearchTreeManager searchTreeManager;
        private readonly IHeuristicTables<TMove> heuristicTables;
        private readonly ICacheTables<TPosition, TMove> cacheTables;

        public AlphaBetaPruning(
            IRules<TPosition, TMove> rules,
            ISearchTreeManager searchTreeManager,
            IHeuristicTables<TMove> heuristicTables,
            ICacheTables<TPosition, TMove> cacheTables)
        {
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            this.searchTreeManager = searchTreeManager ?? throw new ArgumentNullException(nameof(searchTreeManager));
            this.heuristicTables = heuristicTables ?? throw new ArgumentNullException(nameof(heuristicTables));
            this.cacheTables = cacheTables ?? throw new ArgumentNullException(nameof(cacheTables));
        }

        public int Search(TPosition position, int initialAlpha, int initialBeta)
        {
            return SearchRecursively(position, searchTreeManager.StandardDepthLimit, initialAlpha, initialBeta);
        }

        private int SearchRecursively(TPosition position, int depth, int alpha, int beta)
        {
            int originalAlpha = alpha;

            if (cacheTables.TryGetTransposition(position, out Transposition<TMove> transposition) && 
                depth <= transposition.Depth)
            {
                switch (transposition.EvaluationMode)
                {
                    case EvaluationMode.ExactValue:
                        return transposition.Value;
                    case EvaluationMode.LowerBound:
                        alpha = Math.Max(alpha, transposition.Value);
                        break;
                    case EvaluationMode.UpperBound:
                        beta = Math.Min(beta, transposition.Value);
                        break;
                }
                if (IsBetaCutOff(alpha, beta))
                {
                    HandleBetaCutOff(transposition.BestStrategy, depth);
                    return transposition.Value;
                }
            }
            if (IsLeaf(position, depth))
            {
                return -HeuristicValue(position, depth);
            }
            int bestValue = int.MinValue;
            TMove besTMove = default(TMove);

            foreach (var move in OrderedStrategies(position, depth))
            {
                position.Take(move);
                int value = -SearchRecursively(position, depth - 1, -beta, -alpha);
                position.TakeBack();

                if (searchTreeManager.IsStopRequested())
                {
                    return 0;
                }

                if (value > bestValue)
                {
                    bestValue = value;
                    besTMove = move;
                }
                alpha = Math.Max(alpha, value);
                if (IsBetaCutOff(alpha, beta))
                {
                    HandleBetaCutOff(move, depth);
                    break;
                }
            }
            if (depth > 0)
            {
                EvaluationMode evaluationMode = GetEvaluationMode(bestValue, originalAlpha, beta);
                transposition = new Transposition<TMove>(evaluationMode, bestValue, depth, besTMove);
                cacheTables.AddTransposition(position, transposition);
            }
            return bestValue;
        }

        private bool IsBetaCutOff(int alpha, int beta)
        {
            return alpha >= beta;
        }

        private bool IsQuiescenceExtension(int depth)
        {
            return depth < 0;
        }

        private void HandleBetaCutOff(TMove move, int depth)
        {
            if (!searchTreeManager.IsStopRequested() && !IsQuiescenceExtension(depth))
            {
                heuristicTables.StoreBetaCutOff(move, depth);
            }
        }

        private EvaluationMode GetEvaluationMode(int value, int alpha, int beta)
        {
            if (value <= alpha)
            {
                return EvaluationMode.UpperBound;
            }
            else if (value >= beta)
            {
                return EvaluationMode.LowerBound;
            }
            else
            {
                return EvaluationMode.ExactValue;
            }
        }

        private IEnumerable<TMove> OrderedStrategies(TPosition position, int depth)
        {
            var plies = rules.LegalMovesAt(position).ToList();
            Transposition<TMove> transposition;
            if (cacheTables.TryGetTransposition(position, out transposition))
            {
                yield return transposition.BestStrategy;
                plies.Remove(transposition.BestStrategy);
            }
            var killers = heuristicTables.GetKillerPlies(depth);

            var cutPlies = new List<KeyValuePair<TMove, int>>();
            foreach (var move in plies)
            {
                if (killers.Contains(move))
                {
                    yield return move;
                }
                else
                {
                    var score = heuristicTables.GetHistoryScore(move);
                    cutPlies.Add(new KeyValuePair<TMove, int>(move, score));
                }
            }
            foreach (var move in cutPlies.OrderByDescending(p => p.Value))
            {
                yield return move.Key;
            }
        }

        private bool IsLeaf(TPosition position, int depth)
        {
            if (!rules.LegalMovesAt(position).Any())
            {
                return true;
            }
            if (depth <= 0 && position.IsQuiet)
            {
                return true;
            }
            return depth <= -searchTreeManager.QuiescenceDepthLimit;
        }

        private int HeuristicValue(TPosition position, int depth)
        {
            int distanceFromRoot = searchTreeManager.StandardDepthLimit - depth;
            if (rules.LegalMovesAt(position).Any())
            {
                int value;
                if (!cacheTables.TryGetValue(position, out value))
                {
                    value = position.Value;
                    CheckEvaluationValue(value);
                    cacheTables.AddValue(position, value);
                }
                return IsOpponentsTurn(distanceFromRoot) ? -value : value;
            }
            return rules.IsGoal(position) ? int.MaxValue - distanceFromRoot : 0;
        }

        private void CheckEvaluationValue(int value)
        {
            if (value >= int.MaxValue - searchTreeManager.StandardDepthLimit || 
                value <= -int.MaxValue + searchTreeManager.StandardDepthLimit)
            {
                throw new ArgumentOutOfRangeException("value", value, "Invalid static evaluation value");
            }
        }

        private bool IsOpponentsTurn(int distanceFromRoot)
        {
            return distanceFromRoot % 2 != 0;
        }
    }
}