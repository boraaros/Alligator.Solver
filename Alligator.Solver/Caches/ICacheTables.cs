using System;

namespace Alligator.Solver.Caches
{
    internal interface ICacheTables<TPosition, TMove>
    {
        void AddValue(TPosition position, int value);
        bool TryGetValue(TPosition position, out int value);
        void AddTransposition(TPosition position, Transposition<TMove> transposition);
        bool TryGetTransposition(TPosition position, out Transposition<TMove> transposition);
    }
}