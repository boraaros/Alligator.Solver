using Alligator.Solver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alligator.TicTacToe
{
    public class TicTacToePosition : IPosition<TicTacToeCell>
    {
        private readonly TicTacToeMark[,] board;
        private TicTacToeMark nextMarkType;
        
        public const int BoardSize = 3;

        public TicTacToePosition()
        {
            Identifier = 0ul;
            IsEnded = false;
            HasWinner = false;

            board = new TicTacToeMark[BoardSize, BoardSize];
            History = new List<TicTacToeCell>();
            nextMarkType = TicTacToeMark.X;
        }

        public TicTacToePosition(IEnumerable<TicTacToeCell> history)
            : this()
        {
            foreach (var move in history)
            {
                Take(move);
            }
        }

        public ulong Identifier { get; private set; }

        public bool IsEnded { get; private set; }

        public bool HasWinner { get; private set; }

        public IList<TicTacToeCell> History { get; }

        public bool IsQuiet => true;

        public int Value => 0;

        public void Take(TicTacToeCell move)
        {
            if (move == null)
            {
                throw new ArgumentNullException("ply");
            }
            if (IsEnded)
            {
                throw new InvalidOperationException("Cannot mark, because the game is already over");
            }
            if (HasWinner)
            {
                throw new InvalidOperationException(string.Format("Position has winner, but the game isn't over"));
            }
            if (board[move.Row, move.Column] != TicTacToeMark.Empty)
            {
                throw new InvalidOperationException(string.Format("Cannot mark, because target cell isn't empty: [{0},{1}]",
                    move.Row, move.Column));
            }
            board[move.Row, move.Column] = nextMarkType;
            History.Add(move);
            nextMarkType = ChangeMark(nextMarkType);
            Update();
            Identifier = ComputeIdentifier();
        }

        public void TakeBack()
        {
            if (History.Count == 0)
            {
                throw new InvalidOperationException("Cannot remove mark from empty board");
            }
            var lasTMove = History[History.Count - 1];
            if (board[lasTMove.Row, lasTMove.Column] == TicTacToeMark.Empty)
            {
                throw new InvalidOperationException(string.Format("Cannot remove mark, because target cell is already empty: [{0},{1}]",
                    lasTMove.Row, lasTMove.Column));
            }
            board[lasTMove.Row, lasTMove.Column] = TicTacToeMark.Empty;
            History.RemoveAt(History.Count - 1);
            IsEnded = false;
            HasWinner = false;
            nextMarkType = ChangeMark(nextMarkType);
            Identifier = ComputeIdentifier();
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
            var lasTMove = History[History.Count - 1];

            if (IsHorizontalLine(lasTMove.Row) || IsVerticalLine(lasTMove.Column))
            {
                IsEnded = true;
                HasWinner = true;
                return;
            }
            if (board[1, 1] == TicTacToeMark.Empty)
            {
                return;
            }
            if (IsDiagonalLine() || IsReverseDiagonalLine())
            {
                IsEnded = true;
                HasWinner = true;
                return;
            }
            if (!HasEmptyCell())
            {
                IsEnded = true;
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
