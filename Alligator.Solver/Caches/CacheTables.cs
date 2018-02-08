using System;

namespace Alligator.Solver.Caches
{
    class CacheTables<TPosition, TPly> : ICacheTables<TPosition, TPly>
        where TPosition : IPosition<TPly>
    {
        private readonly IHashTable<ulong, int> evaluationTable;
        private readonly IHashTable<ulong, Transposition<TPly>> transpositionTable;

        public CacheTables(ISolverConfiguration solverConfiguration)
        {
            evaluationTable = new HashTable<int>(
                (int)Math.Pow(2, solverConfiguration.EvaluationTableSizeExponent),
                solverConfiguration.EvaluationTableRetryLimit,
                (x, y) => true);
            transpositionTable = new HashTable<Transposition<TPly>>(
                (int)Math.Pow(2, solverConfiguration.TranspositionTableSizeExponent),
                solverConfiguration.TranspositionTableRetryLimit, 
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

        public void AddTransposition(TPosition position, Transposition<TPly> transposition)
        {
            transpositionTable.TryAdd(position.Identifier, transposition);
        }

        public bool TryGetTransposition(TPosition position, out Transposition<TPly> transposition)
        {
            return transpositionTable.TryGetValue(position.Identifier, out transposition);
        }
    }
}
