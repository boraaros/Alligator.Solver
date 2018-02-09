using System;
using System.Collections.Generic;

namespace Alligator.Solver.Heuristics
{
    interface IHeuristicTables<TPly>
    {
        void ClearTables();
        void StoreBetaCutOff(TPly ply, int depth);
        int GetHistoryScore(TPly ply);
        IEnumerable<TPly> GetKillerPlies(int depth);
    }
}
