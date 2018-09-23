using System;
using System.Collections.Generic;
using System.Linq;

namespace Alligator.Solver.UnitTests.Algorithm
{
    internal class TestPosition : IPosition<TesTMove>
    {
        private readonly IList<TesTMove> history;

        private readonly Func<ulong, bool> isEnded;
        private readonly Func<ulong, bool> hasWinner;
        private readonly Func<ulong, bool> isQuiet;

        public TestPosition(Func<ulong, bool> isEnded, Func<ulong, bool> hasWinner, Func<ulong, bool> isQuiet)
        {
            history = new List<TesTMove>();

            this.isEnded = isEnded;
            this.hasWinner = hasWinner;
            this.isQuiet = isQuiet;
        }

        public ulong Identifier
        {
            get { return history.Aggregate(0ul, (sum, next) => sum + next.Value); }
        }

        public bool IsEnded
        {
            get { return isEnded(Identifier); }
        }

        public bool HasWinner
        {
            get { return hasWinner(Identifier); }
        }

        public bool IsQuiet
        {
            get { return isQuiet(Identifier); }
        }

        public int Value => (int)Identifier;

        public void Take(TesTMove move)
        {
            history.Add(move);
        }

        public void TakeBack()
        {
            history.RemoveAt(history.Count - 1);
        }
    }
}
