using System;
using System.Collections.Generic;

namespace Alligator.Solver
{
    public interface ISolver<TPly>
    {
        int Maximize(IList<TPly> history, out IList<TPly> forecast);
    }
}
