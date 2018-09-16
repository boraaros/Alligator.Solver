using System;
using System.Linq;
using System.Collections.Generic;
using Alligator.Solver.Caches;
using Alligator.Solver.Heuristics;

namespace Alligator.Solver.Algorithm
{
    internal class NegaScout<TPosition, TMove> : IMiniMax<TPosition>
        where TPosition : IPosition<TMove>
    {
        private readonly IRules<TPosition, TMove> rules;
        private readonly IHeuristicTables<TMove> heuristicTables;
        private readonly ICacheTables<TPosition, TMove> cacheTables;
        private readonly MiniMaxSettings miniMaxSettings;

        private bool isStopRequested;

        public NegaScout(
            IRules<TPosition, TMove> rules,
            ICacheTables<TPosition, TMove> cacheTables,
            IHeuristicTables<TMove> heuristicTables,
            MiniMaxSettings miniMaxSettings)
        {
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            this.cacheTables = cacheTables ?? throw new ArgumentNullException(nameof(cacheTables));
            this.heuristicTables = heuristicTables ?? throw new ArgumentNullException(nameof(heuristicTables));
            this.miniMaxSettings = miniMaxSettings ?? throw new ArgumentNullException(nameof(miniMaxSettings));
        }

        public int Start(TPosition position, int initialAlpha, int initialBeta)
        {
            isStopRequested = false;
            return SearchRecursively(position, miniMaxSettings.MaxDepth, initialAlpha, initialBeta);
        }

        public int Start(TPosition position)
        {
            return Start(position, -int.MaxValue, int.MaxValue);
        }

        private int SearchRecursively(TPosition position, int depth, int alpha, int beta)
        {
            int originalAlpha = alpha;
            Transposition<TMove> transposition;
            if (cacheTables.TryGetTransposition(position, out transposition) && depth <= transposition.Depth)
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
            bool isFirst = true;
            foreach (var move in OrderedStrategies(position, depth))
            {
                position.Take(move);
                int value = 0;
                if (isFirst)
                {
                    value = -SearchRecursively(position, depth - 1, -beta, -alpha);
                    isFirst = false;
                }
                else
                {
                    value = -SearchRecursively(position, depth - 1, -alpha - 1, -alpha);
                    if (alpha < value && value < beta)
                    {
                        value = -SearchRecursively(position, depth - 1, -beta, -value);
                    }
                }
                position.TakeBack();

                if (isStopRequested)
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
            if (!isStopRequested && !IsQuiescenceExtension(depth))
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
            return depth <= -miniMaxSettings.MaxQuiescenceDepth;
        }

        private int HeuristicValue(TPosition position, int depth)
        {
            int distanceFromRoot = miniMaxSettings.MaxDepth - depth;
            if (rules.LegalMovesAt(position).Any())
            {
                int value;
                if (!cacheTables.TryGetValue(position, out value))
                {
                    value = position.Value;
                    CheckEvaluationValue(value);
                    cacheTables.AddValue(position, value);
                }
                return IsOpponentsTurn(distanceFromRoot) ? value : -value;
            }
            return rules.IsDraw(position) ? 0 : int.MaxValue - distanceFromRoot;
        }

        private void CheckEvaluationValue(int value)
        {
            if (value >= int.MaxValue - miniMaxSettings.MaxDepth || value <= -int.MaxValue + miniMaxSettings.MaxDepth)
            {
                throw new ArgumentOutOfRangeException("value", value, "Invalid evaluation value");
            }
        }

        private bool IsOpponentsTurn(int distanceFromRoot)
        {
            return distanceFromRoot % 2 != 0;
        }

        public void Stop()
        {
            isStopRequested = true;
        }
    }
}
