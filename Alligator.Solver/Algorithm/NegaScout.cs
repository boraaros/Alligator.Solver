﻿using System;
using System.Linq;
using System.Collections.Generic;
using Alligator.Solver.Caches;
using Alligator.Solver.Heuristics;

namespace Alligator.Solver.Algorithm
{
    internal class NegaScout<TPosition, TPly> : IMiniMax<TPosition>
        where TPosition : IPosition<TPly>
    {
        private readonly IExternalLogics<TPosition, TPly> externalLogics;
        private readonly IHeuristicTables<TPly> heuristicTables;
        private readonly ICacheTables<TPosition, TPly> cacheTables;
        private readonly MiniMaxSettings miniMaxSettings;

        private bool isStopRequested;

        public NegaScout(
            IExternalLogics<TPosition, TPly> externalLogics,
            ICacheTables<TPosition, TPly> cacheTables,
            IHeuristicTables<TPly> heuristicTables,
            MiniMaxSettings miniMaxSettings)
        {
            this.externalLogics = externalLogics ?? throw new ArgumentNullException(nameof(externalLogics));
            this.cacheTables = cacheTables ?? throw new ArgumentNullException(nameof(cacheTables));
            this.heuristicTables = heuristicTables ?? throw new ArgumentNullException(nameof(heuristicTables));
            this.miniMaxSettings = miniMaxSettings ?? throw new ArgumentNullException(nameof(miniMaxSettings));
        }

        public int Search(TPosition position, int initialAlpha, int initialBeta)
        {
            isStopRequested = false;
            return SearchRecursively(position, miniMaxSettings.MaxDepth, initialAlpha, initialBeta);
        }

        public int Search(TPosition position)
        {
            return Search(position, -int.MaxValue, int.MaxValue);
        }

        private int SearchRecursively(TPosition position, int depth, int alpha, int beta)
        {
            int originalAlpha = alpha;
            Transposition<TPly> transposition;
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
            TPly bestPly = default(TPly);
            bool isFirst = true;
            foreach (var ply in OrderedStrategies(position, depth))
            {
                position.Do(ply);
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
                position.Undo();

                if (isStopRequested)
                {
                    return 0;
                }

                if (value > bestValue)
                {
                    bestValue = value;
                    bestPly = ply;
                }
                alpha = Math.Max(alpha, value);
                if (IsBetaCutOff(alpha, beta))
                {
                    HandleBetaCutOff(ply, depth);
                    break;
                }
            }
            if (depth > 0)
            {
                EvaluationMode evaluationMode = GetEvaluationMode(bestValue, originalAlpha, beta);
                transposition = new Transposition<TPly>(evaluationMode, bestValue, depth, bestPly);
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

        private void HandleBetaCutOff(TPly ply, int depth)
        {
            if (!isStopRequested && !IsQuiescenceExtension(depth))
            {
                heuristicTables.StoreBetaCutOff(ply, depth);
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

        private IEnumerable<TPly> OrderedStrategies(TPosition position, int depth)
        {
            var plies = externalLogics.GetStrategiesFrom(position).ToList();
            Transposition<TPly> transposition;
            if (cacheTables.TryGetTransposition(position, out transposition))
            {
                yield return transposition.BestStrategy;
                plies.Remove(transposition.BestStrategy);
            }
            var killers = heuristicTables.GetKillerPlies(depth);

            var cutPlies = new List<KeyValuePair<TPly, int>>();
            foreach (var ply in plies)
            {
                if (killers.Contains(ply))
                {
                    yield return ply;
                }
                else
                {
                    var score = heuristicTables.GetHistoryScore(ply);
                    cutPlies.Add(new KeyValuePair<TPly, int>(ply, score));
                }
            }
            foreach (var ply in cutPlies.OrderByDescending(p => p.Value))
            {
                yield return ply.Key;
            }
        }

        private bool IsLeaf(TPosition position, int depth)
        {
            if (position.IsEnded)
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
            if (!position.IsEnded)
            {
                int value;
                if (!cacheTables.TryGetValue(position, out value))
                {
                    value = externalLogics.StaticEvaluate(position);
                    CheckEvaluationValue(value);
                    cacheTables.AddValue(position, value);
                }
                return IsOpponentsTurn(distanceFromRoot) ? value : -value;
            }
            return position.HasWinner ? (int.MaxValue - distanceFromRoot) : 0;
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
