using Alligator.Solver.Heuristics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alligator.Solver.UnitTests.Heuristics
{
    [TestClass]
    public class HeuristicTablesTests
    {
        private readonly IList<string> testMoves = Enumerable.Range(0, 3)
            .Select(t => string.Format("testMove{0}", t)).ToList();

        [TestMethod]
        public void Heuristic_tables_store_and_return_killer_plies_with_same_depth()
        {
            // Arrange
            var heuristicTables = new HeuristicTables<string>();

            // Act
            heuristicTables.StoreBetaCutOff(testMoves[0], 1);
            heuristicTables.StoreBetaCutOff(testMoves[1], 1);
            IList<string> result = heuristicTables.GetKillerPlies(1).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(testMoves[0]));
            Assert.IsTrue(result.Contains(testMoves[1]));
        }

        [TestMethod]
        public void Heuristic_tables_does_not_return_killer_plies_with_different_depth()
        {
            // Arrange
            var heuristicTables = new HeuristicTables<string>();

            // Act
            heuristicTables.StoreBetaCutOff(testMoves[0], 1);
            heuristicTables.StoreBetaCutOff(testMoves[1], 2);
            IList<string> result = heuristicTables.GetKillerPlies(1).ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.Contains(testMoves[0]));
        }

        [TestMethod]
        public void Heuristic_tables_return_only_last_two_stored_killer_plies()
        {
            // Arrange
            var heuristicTables = new HeuristicTables<string>();

            // Act
            heuristicTables.StoreBetaCutOff(testMoves[0], 1);
            heuristicTables.StoreBetaCutOff(testMoves[1], 1);
            heuristicTables.StoreBetaCutOff(testMoves[2], 1);
            IList<string> result = heuristicTables.GetKillerPlies(1).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(testMoves[1]));
            Assert.IsTrue(result.Contains(testMoves[2]));
        }

        [TestMethod]
        public void Heuristic_tables_store_and_return_only_different_killer_plies()
        {
            // Arrange
            var heuristicTables = new HeuristicTables<string>();

            // Act
            heuristicTables.StoreBetaCutOff(testMoves[0], 1);
            heuristicTables.StoreBetaCutOff(testMoves[1], 1);
            heuristicTables.StoreBetaCutOff(testMoves[1], 1);
            heuristicTables.StoreBetaCutOff(testMoves[1], 1);
            IList<string> result = heuristicTables.GetKillerPlies(1).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(testMoves[0]));
            Assert.IsTrue(result.Contains(testMoves[1]));
        }

        [TestMethod]
        public void Heuristic_tables_does_not_return_killer_plies_after_clear()
        {
            // Arrange
            var heuristicTables = new HeuristicTables<string>();

            // Act
            heuristicTables.StoreBetaCutOff(testMoves[0], 1);
            heuristicTables.StoreBetaCutOff(testMoves[1], 1);
            heuristicTables.ClearTables();
            IList<string> result = heuristicTables.GetKillerPlies(1).ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Heuristic_tables_calculate_history_score()
        {
            // Arrange
            var heuristicTables = new HeuristicTables<string>();

            // Act
            heuristicTables.StoreBetaCutOff(testMoves[0], 1);
            heuristicTables.StoreBetaCutOff(testMoves[0], 2);
            int result = heuristicTables.GetHistoryScore(testMoves[0]);

            // Assert
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void Heuristic_tables_does_not_update_history_score_with_another_ply()
        {
            // Arrange
            var heuristicTables = new HeuristicTables<string>();

            // Act
            heuristicTables.StoreBetaCutOff(testMoves[0], 1);
            heuristicTables.StoreBetaCutOff(testMoves[1], 2);
            int result = heuristicTables.GetHistoryScore(testMoves[0]);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Heuristic_tables_return_zero_history_score_after_clear()
        {
            // Arrange
            var heuristicTables = new HeuristicTables<string>();

            // Act
            heuristicTables.StoreBetaCutOff(testMoves[0], 1);
            heuristicTables.ClearTables();
            int result = heuristicTables.GetHistoryScore(testMoves[0]);

            // Assert
            Assert.AreEqual(0, result);
        }
    }
}
