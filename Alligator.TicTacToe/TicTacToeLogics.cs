using Alligator.Solver;
using System;
using System.Collections.Generic;

namespace Alligator.TicTacToe
{
    public class TicTacToeLogics : IExternalLogics<ITicTacToePosition, TicTacToeCell>
    {
        public ITicTacToePosition CreateEmptyPosition()
        {
            return new TicTacToePosition();
        }

        public IEnumerable<TicTacToeCell> GetStrategiesFrom(ITicTacToePosition position)
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

        public int StaticEvaluate(ITicTacToePosition position)
        {
            CheckPosition(position);
            return 0;
        }

        private void CheckPosition(ITicTacToePosition position)
        {
            if (position == null)
            {
                throw new ArgumentNullException("position");
            }
            if (position.IsEnded)
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
