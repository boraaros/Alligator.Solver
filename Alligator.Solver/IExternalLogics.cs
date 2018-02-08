using System;
using System.Collections.Generic;

namespace Alligator.Solver
{
    public interface IExternalLogics<TPosition, TPly>
    {
        TPosition CreateEmptyPosition();
        IEnumerable<TPly> Strategies(TPosition position);
        int Evaluate(TPosition position);
    }
}
