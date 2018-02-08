using System;
using System.Collections.Generic;
using System.Linq;

namespace Alligator.TicTacToe
{
    public class TicTacToePosition : ITicTacToePosition
    {
        private ulong identifier;
        private bool isOver;
        private bool hasWinner;

        private readonly TicTacToeMark[,] board;
        private readonly IList<TicTacToeCell> history;
        private TicTacToeMark nextMarkType;
        
        public const int BoardSize = 3;

        public TicTacToePosition()
        {
            identifier = 0ul;
            isOver = false;
            hasWinner = false;

            board = new TicTacToeMark[BoardSize, BoardSize];
            history = new List<TicTacToeCell>();
            nextMarkType = TicTacToeMark.X;
        }

        public TicTacToePosition(IEnumerable<TicTacToeCell> history)
            : this()
        {
            foreach (var ply in history)
            {
                Do(ply);
            }
        }

        public ulong Identifier
        {
            get { return identifier; }
        }

        public bool IsOver
        {
            get { return isOver; }
        }

        public bool HasWinner
        {
            get { return hasWinner; }
        }

        public bool IsQuiet
        {
            get { return true; }
        }

        public void Do(TicTacToeCell ply)
        {
            if (ply == null)
            {
                throw new ArgumentNullException("ply");
            }
            if (isOver)
            {
                throw new InvalidOperationException("Cannot mark, because the game is already over");
            }
            if (hasWinner)
            {
                throw new InvalidOperationException(string.Format("Position has winner, but the game isn't over"));
            }
            if (board[ply.Row, ply.Column] != TicTacToeMark.Empty)
            {
                throw new InvalidOperationException(string.Format("Cannot mark, because target cell isn't empty: [{0},{1}]",
                    ply.Row, ply.Column));
            }
            board[ply.Row, ply.Column] = nextMarkType;
            history.Add(ply);
            nextMarkType = ChangeMark(nextMarkType);
            Update();
            identifier = ComputeIdentifier();
        }

        public void Undo()
        {
            if (history.Count == 0)
            {
                throw new InvalidOperationException("Cannot remove mark from empty board");
            }
            var lastPly = history[history.Count - 1];
            if (board[lastPly.Row, lastPly.Column] == TicTacToeMark.Empty)
            {
                throw new InvalidOperationException(string.Format("Cannot remove mark, because target cell is already empty: [{0},{1}]",
                    lastPly.Row, lastPly.Column));
            }
            board[lastPly.Row, lastPly.Column] = TicTacToeMark.Empty;
            history.RemoveAt(history.Count - 1);
            isOver = false;
            hasWinner = false;
            nextMarkType = ChangeMark(nextMarkType);
            identifier = ComputeIdentifier();
        }

        public TicTacToeMark GetMarkAt(int row, int column)
        {
            return board[row, column];
        }

        public ulong ComputeIdentifier()
        {
            var hashCode = 0ul;
            var exp = 0;

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (board[i, j] == TicTacToeMark.X)
                    {
                        hashCode += (ulong)Math.Pow(2, exp);
                    }
                    exp++;
                    if (board[i, j] == TicTacToeMark.O)
                    {
                        hashCode += (ulong)Math.Pow(2, exp);
                    }
                    exp++;
                }
            }
            return hashCode;
        }

        private void Update()
        {
            var lastPly = history[history.Count - 1];

            if (IsHorizontalLine(lastPly.Row) || IsVerticalLine(lastPly.Column))
            {
                isOver = true;
                hasWinner = true;
                return;
            }
            if (board[1, 1] == TicTacToeMark.Empty)
            {
                return;
            }
            if (IsDiagonalLine() || IsReverseDiagonalLine())
            {
                isOver = true;
                hasWinner = true;
                return;
            }
            if (!HasEmptyCell())
            {
                isOver = true;
            }
        }

        private bool IsHorizontalLine(int row)
        {
            return Enumerable.Range(0, BoardSize).Select(t => board[row, t]).Distinct().Count() == 1;
        }

        private bool IsVerticalLine(int column)
        {
            return Enumerable.Range(0, BoardSize).Select(t => board[t, column]).Distinct().Count() == 1;
        }

        private bool IsDiagonalLine()
        {
            return Enumerable.Range(0, BoardSize).Select(t => board[t, t]).Distinct().Count() == 1;
        }

        private bool IsReverseDiagonalLine()
        {
            return Enumerable.Range(0, BoardSize).Select(t => board[t, BoardSize - 1 - t]).Distinct().Count() == 1;
        }

        private bool HasEmptyCell()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (board[i, j] == TicTacToeMark.Empty)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private TicTacToeMark ChangeMark(TicTacToeMark Mark)
        {
            if (Mark == TicTacToeMark.Empty)
            {
                throw new ArgumentOutOfRangeException("Cannot change empty mark type");
            }
            return Mark == TicTacToeMark.X ? TicTacToeMark.O : TicTacToeMark.X;
        }
    }
}
