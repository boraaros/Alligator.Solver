using System;

namespace Alligator.Solver.Algorithm
{
    internal interface IMiniMax<TPosition>
    {
        int Start(TPosition position);
        int Start(TPosition position, int initialAlpha, int initialBeta);
        void Stop();
    }
}