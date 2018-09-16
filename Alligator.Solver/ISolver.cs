using System;
using System.Collections.Generic;

namespace Alligator.Solver
{
    /// <summary>
    /// The main interface of the abstract solver component.
    /// </summary>
    /// <typeparam name="TMove">type of moves in the specified game</typeparam>
    public interface ISolver<TMove>
    {
        /// <summary>
        /// Calculate the optimal next move from the game position defined by move history.
        /// </summary>
        /// <param name="moveHistory">ordered list of all previous move in the current game</param>
        /// <returns>the optimal next move</returns>
        TMove CalculateNextMove(IList<TMove> moveHistory);
    }
}