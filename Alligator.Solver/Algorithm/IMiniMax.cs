using System;

namespace Alligator.Solver.Algorithm
{
    internal interface IMiniMax<TPosition>
    {
        int Search(TPosition position);
        int Search(TPosition position, int initialAlpha, int initialBeta);
        void Stop();
    }
}
