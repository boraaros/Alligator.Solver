using System;

namespace Alligator.Solver
{
    /// <summary>
    /// Represent the game board.
    /// </summary>
    /// <typeparam name="TMove">type of moves in the specified game</typeparam>
    public interface IPosition<TMove>
    {
        /// <summary>
        /// Unique identifier or very strong hash.
        /// </summary>
        ulong Identifier { get; }

        /// <summary>
        /// Static evaluation value.
        /// </summary>
        int Value { get; }

        /// <summary>
        /// This game specified logics can reduce the horizon effect.
        /// </summary>
        bool IsQuiet { get; }    

        /// <summary>
        /// Take the specified move.
        /// </summary>
        /// <param name="move">specified move</param>
        void Take(TMove move);

        /// <summary>
        /// Take back the last move.
        /// </summary>
        void TakeBack();
    }
}