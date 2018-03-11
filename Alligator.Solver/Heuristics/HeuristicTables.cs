using System;
using System.Collections.Generic;
using System.Linq;

namespace Alligator.Solver.Heuristics
{
    internal class HeuristicTables<TPly> : IHeuristicTables<TPly>
    {
        private readonly IDictionary<int, IList<TPly>> killerPlies;
        private readonly IDictionary<TPly, int> historyScores;

        private const int KillerPliesLimitPerDepth = 2;

        public HeuristicTables()
        {
            killerPlies = new Dictionary<int, IList<TPly>>();
            historyScores = new Dictionary<TPly, int>();
        }

        public void ClearTables()
        {
            killerPlies.Clear();
            historyScores.Clear();
        }

        public void StoreBetaCutOff(TPly ply, int depth)
        {
            UpdateHistoryScores(ply, depth);
            UpdateKillerPlies(ply, depth);
        }

        public int GetHistoryScore(TPly ply)
        {
            int result;
            if (historyScores.TryGetValue(ply, out result))
            {
                return result;
            }
            return 0;
        }

        public IEnumerable<TPly> GetKillerPlies(int depth)
        {
            IList<TPly> killers;
            if (killerPlies.TryGetValue(depth, out killers))
            {
                return killers;
            }
            return Enumerable.Empty<TPly>();
        }

        public void UpdateHistoryScores(TPly ply, int depth)
        {
            var score = depth * depth;
            int result;
            if (historyScores.TryGetValue(ply, out result))
            {
                historyScores[ply] = result + score;
            }
            else
            {
                historyScores.Add(ply, score);
            }
        }

        public void UpdateKillerPlies(TPly ply, int depth)
        {
            IList<TPly> killers;
            if (killerPlies.TryGetValue(depth, out killers))
            {
                if (killers[0].Equals(ply))
                {
                    return;
                }
                killers.Insert(0, ply);
                if (killers.Count > KillerPliesLimitPerDepth)
                {
                    killers.RemoveAt(KillerPliesLimitPerDepth);
                }
            }
            else
            {
                killerPlies.Add(depth, new List<TPly> { ply });
            }
        }
    }
}
