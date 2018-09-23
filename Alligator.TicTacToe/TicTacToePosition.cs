using Alligator.Solver;
using System;
using System.Collections.Generic;

namespace Alligator.TicTacToe
{
    public class TicTacToePosition : IPosition<TicTacToeCell>
    {
        private readonly TicTacToeMark[,] board;
        private TicTacToeMark nextMarkType;

        public ulong Identifier { get; private set; }
        public IList<TicTacToeCell> History { get; }
        public bool IsQuiet => true;
        public int Value => 0;

        public const int BoardSize = 3;

        public TicTacToePosition()
        {
            Identifier = 0ul;
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

        public void Take(TicTacToeCell move)
        {
            if (move == null)
            {
                throw new ArgumentNullException(nameof(move));
            }
            if (board[move.Row, move.Column] != TicTacToeMark.Empty)
            {
                throw new InvalidOperationException(string.Format("Cannot mark, because target cell isn't empty: [{0},{1}]",
                    move.Row, move.Column));
            }
            board[move.Row, move.Column] = nextMarkType;
            History.Add(move);
            nextMarkType = ChangeMark(nextMarkType);
            Identifier = ComputeIdentifier();
        }

        public void TakeBack()
        {
            if (History.Count == 0)
            {
                throw new InvalidOperationException("Cannot remove last mark from empty board");
            }
            var lasTMove = History[History.Count - 1];
            if (board[lasTMove.Row, lasTMove.Column] == TicTacToeMark.Empty)
            {
                throw new InvalidOperationException($"Cannot remove mark, because target cell is already empty: [{lasTMove.Row},{lasTMove.Column}]");
            }
            board[lasTMove.Row, lasTMove.Column] = TicTacToeMark.Empty;
            History.RemoveAt(History.Count - 1);
            nextMarkType = ChangeMark(nextMarkType);
            Identifier = ComputeIdentifier();
        }

        public TicTacToeMark GetMarkAt(int row, int column)
        {
            return board[row, column];
        }

        private ulong ComputeIdentifier()
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