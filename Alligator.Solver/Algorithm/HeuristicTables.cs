using System;
using System.Collections.Generic;
using System.Linq;

namespace Alligator.Solver.Algorithm
{
    internal class HeuristicTables<TMove> : IHeuristicTables<TMove>
    {
        private readonly IDictionary<int, IList<TMove>> killerPlies;
        private readonly IDictionary<TMove, int> historyScores;

        private const int KillerPliesLimitPerDepth = 2;

        public HeuristicTables()
        {
            killerPlies = new Dictionary<int, IList<TMove>>();
            historyScores = new Dictionary<TMove, int>();
        }

        public void StoreBetaCutOff(TMove move, int depth)
        {
            UpdateHistoryScores(move, depth);
            UpdateKillerPlies(move, depth);
        }

        public int GetHistoryScore(TMove move)
        {
            int result;
            if (historyScores.TryGetValue(move, out result))
            {
                return result;
            }
            return 0;
        }

        public IEnumerable<TMove> GetKillerPlies(int depth)
        {
            IList<TMove> killers;
            if (killerPlies.TryGetValue(depth, out killers))
            {
                return killers;
            }
            return Enumerable.Empty<TMove>();
        }

        public void UpdateHistoryScores(TMove move, int depth)
        {
            var score = depth * depth;
            int result;
            if (historyScores.TryGetValue(move, out result))
            {
                historyScores[move] = result + score;
            }
            else
            {
                historyScores.Add(move, score);
            }
        }

        public void UpdateKillerPlies(TMove move, int depth)
        {
            IList<TMove> killers;
            if (killerPlies.TryGetValue(depth, out killers))
            {
                if (killers[0].Equals(move))
                {
                    return;
                }
                killers.Insert(0, move);
                if (killers.Count > KillerPliesLimitPerDepth)
                {
                    killers.RemoveAt(KillerPliesLimitPerDepth);
                }
            }
            else
            {
                killerPlies.Add(depth, new List<TMove> { move });
            }
        }
    }
}