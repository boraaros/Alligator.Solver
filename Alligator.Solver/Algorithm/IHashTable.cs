using System;

namespace Alligator.Solver.Algorithm
{
    internal interface IHashTable<TKey, TValue>
    {
        bool TryAdd(TKey key, TValue value);
        bool TryGetValue(TKey key, out TValue value);
    }
}