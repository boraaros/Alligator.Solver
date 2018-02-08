using System;

namespace Alligator.Solver.Caches
{
    interface ICacheTables<TPosition, TPly>
    {
        void AddValue(TPosition position, int value);
        bool TryGetValue(TPosition position, out int value);
        void AddTransposition(TPosition position, Transposition<TPly> transposition);
        bool TryGetTransposition(TPosition position, out Transposition<TPly> transposition);
    }
}
