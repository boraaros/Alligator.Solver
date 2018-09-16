using System;

namespace Alligator.Solver.Caches
{
    internal class CacheTables<TPosition, TMove> : ICacheTables<TPosition, TMove>
        where TPosition : IPosition<TMove>
    {
        private readonly IHashTable<ulong, int> evaluationTable;
        private readonly IHashTable<ulong, Transposition<TMove>> transpositionTable;

        public CacheTables(ICachesSettings cachesSettings)
        {
            evaluationTable = new HashTable<int>(
                (int)Math.Pow(2, cachesSettings.EvaluationTableSizeExponent),
                cachesSettings.EvaluationTableRetryLimit,
                (x, y) => true);
            transpositionTable = new HashTable<Transposition<TMove>>(
                (int)Math.Pow(2, cachesSettings.TranspositionTableSizeExponent),
                cachesSettings.TranspositionTableRetryLimit, 
                (x, y) => true);
        }

        public void AddValue(TPosition position, int value)
        {
            evaluationTable.TryAdd(position.Identifier, value);
        }

        public bool TryGetValue(TPosition position, out int value)
        {
            return evaluationTable.TryGetValue(position.Identifier, out value);
        }

        public void AddTransposition(TPosition position, Transposition<TMove> transposition)
        {
            transpositionTable.TryAdd(position.Identifier, transposition);
        }

        public bool TryGetTransposition(TPosition position, out Transposition<TMove> transposition)
        {
            return transpositionTable.TryGetValue(position.Identifier, out transposition);
        }
    }
}
