using System;

namespace Alligator.Solver.Algorithm
{
    internal interface IAlphaBetaPruning<TPosition>
    {
        int Search(TPosition position, int alpha, int beta);
    }
}