using System;

namespace Alligator.Solver.Caches
{
    internal class Transposition<TMove>
    {
        public EvaluationMode EvaluationMode;
        public int Value;
        public int Depth;
        public TMove BestStrategy;

        public Transposition(EvaluationMode evaluationMode, int value, int depth, TMove bestStrategy)
        {
            EvaluationMode = evaluationMode;
            Value = value;
            Depth = depth;
            BestStrategy = bestStrategy;
        }
    }
}