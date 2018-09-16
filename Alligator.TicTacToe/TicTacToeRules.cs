using Alligator.Solver;
using System;
using System.Collections.Generic;

namespace Alligator.TicTacToe
{
    public class TicTacToeRules : IRules<TicTacToePosition, TicTacToeCell>
    {
        public TicTacToePosition InitialPosition()
        {
            return new TicTacToePosition();
        }

        public IEnumerable<TicTacToeCell> LegalMovesAt(TicTacToePosition position)
        {
            if (position == null)
            {
                throw new ArgumentNullException("position");
            }

            if (position.IsEnded)
            {
                yield break;
            }

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

        public bool IsDraw(TicTacToePosition position)
        {
            if (position == null)
            {
                throw new ArgumentNullException("position");
            }

            return !position.HasWinner;
        }
    }
}