using System;
using System.Collections.Generic;

namespace Alligator.Solver.Algorithm
{
    internal interface IHeuristicTables<TMove>
    {
        void StoreBetaCutOff(TMove move, int depth);
        int GetHistoryScore(TMove move);
        IEnumerable<TMove> GetKillerPlies(int depth);
    }
}