using System;
using System.Collections.Generic;
using System.Linq;

namespace Alligator.Solver.UnitTests.Algorithm
{
    class TestPosition : IPosition<TestPly>
    {
        private readonly IList<TestPly> history;

        private readonly Func<ulong, bool> isEnded;
        private readonly Func<ulong, bool> hasWinner;
        private readonly Func<ulong, bool> isQuiet;

        public TestPosition(Func<ulong, bool> isEnded, Func<ulong, bool> hasWinner, Func<ulong, bool> isQuiet)
        {
            history = new List<TestPly>();

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

        public void Do(TestPly ply)
        {
            history.Add(ply);
        }

        public void Undo()
        {
            history.RemoveAt(history.Count - 1);
        }
    }
}
