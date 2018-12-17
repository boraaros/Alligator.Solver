using System;

namespace Alligator.Solver
{
    /// <summary>
    /// Represents the game board.
    /// </summary>
    /// <typeparam name="TMove">type of moves in the specified game</typeparam>
    public interface IPosition<TMove>
    {
        /// <summary>
        /// Unique identifier or very strong hash.
        /// </summary>
        ulong Identifier { get; }

        /// <summary>
        /// Static evaluation value. TODO: From which player's point of view?
        /// </summary>
        int Value { get; }

        /// <summary>
        /// This game specified logic can reduce the horizon effect.
        /// </summary>
        bool IsQuiet { get; }    

        /// <summary>
        /// Updates the position with the specified move.
        /// </summary>
        /// <param name="move">specified move</param>
        void Take(TMove move);

        /// <summary>
        /// Withdraws the last move. TODO: Remove this method, 'Take' should create new Position instance!
        /// </summary>
        void TakeBack();
    }
}