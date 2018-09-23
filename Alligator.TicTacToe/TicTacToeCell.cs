using System;

namespace Alligator.TicTacToe
{
    public class TicTacToeCell
    {
        public readonly int Row;
        public readonly int Column;

        public TicTacToeCell(int row, int column)
        {
            if (row < 0 || row >= TicTacToePosition.BoardSize || column < 0 || column >= TicTacToePosition.BoardSize)
            {
                throw new ArgumentOutOfRangeException(string.Format("Invalid row ({0}) or column ({1}) index", row, column));
            }
            Row = row;
            Column = column;
        }

        public override int GetHashCode()
        {
            return TicTacToePosition.BoardSize * Row + Column;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            var move = obj as TicTacToeCell;
            if (move == null)
            {
                return false;
            }
            return Row == move.Row && Column == move.Column;
        }

        public override string ToString()
        {
            return string.Format("[{0}:{1}]", Row, Column);
        }
    }
}