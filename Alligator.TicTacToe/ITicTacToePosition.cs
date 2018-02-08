using Alligator.Solver;
using System;

namespace Alligator.TicTacToe
{
    public interface ITicTacToePosition : IPosition<TicTacToeCell>
    {
        TicTacToeMark GetMarkAt(int row, int column);
    }
}
