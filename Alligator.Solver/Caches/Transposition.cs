using System;

namespace Alligator.Solver.Caches
{
    internal class Transposition<TPly>
    {
        public EvaluationMode EvaluationMode;
        public int Value;
        public int Depth;
        public TPly BestStrategy;

        public Transposition(EvaluationMode evaluationMode, int value, int depth, TPly bestStrategy)
        {
            EvaluationMode = evaluationMode;
            Value = value;
            Depth = depth;
            BestStrategy = bestStrategy;
        }
    }
}
