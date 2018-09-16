using System;
using System.Collections.Generic;

namespace Alligator.Solver
{
    /// <summary>
    /// This component provides the rules of the specified game.
    /// </summary>
    /// <typeparam name="TPosition">type of position in the specified game</typeparam>
    /// <typeparam name="TMove">type of moves in the specified game</typeparam>
    public interface IRules<TPosition, TMove>
    {
        /// <summary>
        /// Provide the initial position of the game.
        /// </summary>
        /// <returns>the starting position of the game</returns>
        TPosition InitialPosition();

        /// <summary>
        /// Enumerate the legal moves at the specified game position.
        /// </summary>
        /// <param name="position">specified game position</param>
        /// <returns>the legal moves</returns>
        IEnumerable<TMove> LegalMovesAt(TPosition position);

        /// <summary>
        /// Define the result of the game.
        /// </summary>
        /// <param name="position">specified game position</param>
        /// <returns>true if game is in progress or ended with a tie</returns>
        bool IsDraw(TPosition position);
    }
}