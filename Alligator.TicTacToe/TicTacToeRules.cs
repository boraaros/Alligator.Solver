using Alligator.Solver;
using System;
using System.Collections.Generic;
using System.Linq;

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

            if (IsGoal(position))
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

        public bool IsGoal(TicTacToePosition position)
        {
            if (position == null)
            {
                throw new ArgumentNullException("position");
            }

            if (!position.History.Any())
            {
                return false;
            }

            var lastMove = position.History[position.History.Count - 1];

            if (IsHorizontalLine(position, lastMove.Row) || IsVerticalLine(position, lastMove.Column))
            {
                return true;
            }
            if (position.GetMarkAt(1, 1) == TicTacToeMark.Empty)
            {
                return false;
            }
            if (HasDiagonalLine(position) || HasReverseDiagonalLine(position))
            {
                return true;
            }

            return false;
        }

        private bool IsHorizontalLine(TicTacToePosition position, int row)
        {
            return Enumerable.Range(0, TicTacToePosition.BoardSize)
                .Select(t => position.GetMarkAt(row, t)).Distinct().Count() == 1;
        }

        private bool IsVerticalLine(TicTacToePosition position, int column)
        {
            return Enumerable.Range(0, TicTacToePosition.BoardSize)
                .Select(t => position.GetMarkAt(t, column)).Distinct().Count() == 1;
        }

        private bool HasDiagonalLine(TicTacToePosition position)
        {
            return Enumerable.Range(0, TicTacToePosition.BoardSize)
                .Select(t => position.GetMarkAt(t, t)).Distinct().Count() == 1;
        }

        private bool HasReverseDiagonalLine(TicTacToePosition position)
        {
            return Enumerable.Range(0, TicTacToePosition.BoardSize)
                .Select(t => position.GetMarkAt(t, TicTacToePosition.BoardSize - 1 - t)).Distinct().Count() == 1;
        }
    }
}