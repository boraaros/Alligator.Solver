using Alligator.Solver;
using System;
using System.Collections.Generic;

namespace Alligator.TicTacToe
{
    public class TicTacToeLogics : IExternalLogics<TicTacToePosition, TicTacToeCell>
    {
        public TicTacToePosition CreateEmptyPosition()
        {
            return new TicTacToePosition();
        }

        public IEnumerable<TicTacToeCell> Strategies(TicTacToePosition position)
        {
            CheckPosition(position);

            for (int i = 0; i < TicTacToePosition.BoardSize; i++)
            {
                for (int j = 0; j < TicTacToePosition.BoardSize; j++)
                {
                    if (position.GetMarkAt(i, j) == TicTacToeMark.Empty)
                    {
                        yield return new TicTacToeCell(i, j);
                    }
                }
            }
        }

        public int Evaluate(TicTacToePosition position)
        {
            CheckPosition(position);
            return 0;
        }

        private void CheckPosition(TicTacToePosition position)
        {
            if (position == null)
            {
                throw new ArgumentNullException("position");
            }
            if (position.IsOver)
            {
                throw new InvalidOperationException(string.Format("Cannot evaluate a closed position"));
            }
            if (position.HasWinner)
            {
                throw new InvalidOperationException(string.Format("Position has winner, but doesn't closed"));
            }
        }
    }
}
