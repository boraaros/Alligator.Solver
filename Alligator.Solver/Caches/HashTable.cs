using System;

namespace Alligator.Solver.Caches
{
    public class HashTable<TValue> : IHashTable<ulong, TValue>
    {
        private struct Entry
        {
            public ulong Key;
            public TValue Value;
            public bool Filled;

            public Entry(ulong key, TValue value)
            {
                Key = key;
                Value = value;
                Filled = true;
            }
        }

        private readonly Entry[] table;
        private readonly int retryLimit;
        private readonly Func<TValue, TValue, bool> IsReplaceable;

        private readonly TValue DefaultValue = default(TValue);

        public HashTable(int length, int retryLimit, Func<TValue, TValue, bool> isReplaceable)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException("length", length, "Value must be positive");
            }
            if (retryLimit < 0)
            {
                throw new ArgumentOutOfRangeException("retryLimit", retryLimit, "Value must be non-negative");
            }

            table = new Entry[length];
            this.retryLimit = retryLimit;
            IsReplaceable = isReplaceable ?? throw new ArgumentNullException("isReplaceable");
        }

        public bool TryAdd(ulong key, TValue value)
        {
            int hashCode = key.GetHashCode();
            for (int i = hashCode; i <= hashCode + retryLimit; i++)
            {
                int index = i & (table.Length - 1);
                var item = table[index];

                if (!item.Filled || IsReplaceable(item.Value, value))
                {
                    table[index] = new Entry(key, value);
                    return true;
                }
            }
            return false;
        }

        public bool TryGetValue(ulong key, out TValue value)
        {
            int hashCode = key.GetHashCode();

            for (int i = hashCode; i <= hashCode + retryLimit; i++)
            {
                int index = i & (table.Length - 1);
                var item = table[index];

                if (!item.Filled)
                {
                    break;
                }
                if (item.Key.Equals(key))
                {
                    value = item.Value;
                    return true;
                }
            }
            value = DefaultValue;
            return false;
        }
    }
}
