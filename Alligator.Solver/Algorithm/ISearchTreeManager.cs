using System;

namespace Alligator.Solver.Algorithm
{
    internal interface ISearchTreeManager
    {
        int StandardDepthLimit { get; }
        int QuiescenceDepthLimit { get; }

        void IterationCompleted();

        void Restart();

        bool IsStopRequested();
    }
}