using System;
using System.Collections.Generic;

namespace Alligator.Solver.Algorithm
{
    internal class SearchTreeManager : ISearchTreeManager
    {
        private readonly TimeSpan timeLimitPerMove;
        private readonly IList<DateTime> elapsedTimePerIteration;

        private DateTime startTime;

        public int StandardDepthLimit => elapsedTimePerIteration.Count + 1;

        public int QuiescenceDepthLimit => 0; // TODO: QS-search!

        public SearchTreeManager(TimeSpan timeLimitPerMove)
        {
            this.timeLimitPerMove = timeLimitPerMove;
            elapsedTimePerIteration = new List<DateTime>();
        }

        public bool IsStopRequested()
        {
            return startTime + timeLimitPerMove <= DateTime.Now;
        }

        public void IterationCompleted()
        {
            elapsedTimePerIteration.Add(DateTime.Now);
        }

        public void Restart()
        {
            startTime = DateTime.Now;
            elapsedTimePerIteration.Clear();
        }
    }
}