using System;
using System.Collections.Generic;

namespace Alligator.Solver.Heuristics
{
    internal interface IHeuristicTables<TMove>
    {
        void ClearTables();
        void StoreBetaCutOff(TMove move, int depth);
        int GetHistoryScore(TMove move);
        IEnumerable<TMove> GetKillerPlies(int depth);
    }
}