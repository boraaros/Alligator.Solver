using System;
using System.Collections.Generic;

namespace Alligator.Solver
{
    public interface IExternalLogics<TPosition, TPly>
    {
        TPosition CreateEmptyPosition();
        IEnumerable<TPly> GetStrategiesFrom(TPosition position);
        int StaticEvaluate(TPosition position);
    }
}
