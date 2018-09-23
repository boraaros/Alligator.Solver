using System;

namespace Alligator.Solver.Algorithm
{
    internal class AlgorithmSettings : IAlgorithmSettings
    {
        public int SearchDepthLimit => 32;
        public int QuiescenceExtensionLimit => 0;
        public int MinimumSearchDepthToUseMtdf => 99;
    }
}