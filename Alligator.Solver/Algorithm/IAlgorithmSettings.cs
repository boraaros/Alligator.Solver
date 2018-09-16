using System;

namespace Alligator.Solver.Algorithm
{
    internal interface IAlgorithmSettings
    {
        int SearchDepthLimit { get; }
        int QuiescenceExtensionLimit { get; }
        int MinimumSearchDepthToUseMtdf { get; }
    }
}