using System;

namespace Alligator.Solver.Algorithm
{
    internal class MiniMaxSettings
    {
        public readonly int MaxDepth;
        public readonly int MaxQuiescenceDepth;

        public MiniMaxSettings(int maxDepth, int maxQuiescenceDepth)
        {
            MaxDepth = maxDepth;
            MaxQuiescenceDepth = maxQuiescenceDepth;
        }
    }
}