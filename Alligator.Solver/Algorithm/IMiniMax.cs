using System;

namespace Alligator.Solver.Algorithm
{
    interface IMiniMax<TPosition>
    {
        int Search(TPosition position);
        void Stop();
    }
}
