using Alligator.TicTacToe;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace Alligator.Solver.UnitTests
{
    [TestClass]
    public class ModulTests
    {
        [TestMethod]
        public void Solver_returns_zero_for_tied_tic_tac_toe_position()
        {
            // Arrange

            // . . .
            // . X .
            // O . .
            List<TicTacToeCell> history = new List<TicTacToeCell>()
            {
                new TicTacToeCell(1, 1),
                new TicTacToeCell(2, 0)
            };

            var externalLogics = new TicTacToeLogics();
            var solverConfiguration = GetSolverConfiguration();
            var solverFactory = new SolverFactory<TicTacToePosition, TicTacToeCell>(externalLogics, solverConfiguration);
            ISolver<TicTacToeCell> solver = solverFactory.Create();

            // Act
            IList<TicTacToeCell> forecast;
            int result = solver.Maximize(history, out forecast);

            // Assert
            Assert.AreEqual(0, result);
            Assert.IsNotNull(forecast);
            Assert.AreEqual(7, forecast.Count);
        }

        [TestMethod]
        public void Solver_recognizes_winnable_tic_tac_toe_position()
        {
            // Arrange

            // O X .
            // X . .
            // . . O
            List<TicTacToeCell> history = new List<TicTacToeCell>()
            {
                new TicTacToeCell(1, 0),
                new TicTacToeCell(0, 0),
                new TicTacToeCell(0, 1),
                new TicTacToeCell(2, 2)
            };

            var externalLogics = new TicTacToeLogics();
            var solverConfiguration = GetSolverConfiguration();
            var solverFactory = new SolverFactory<TicTacToePosition, TicTacToeCell>(externalLogics, solverConfiguration);
            ISolver<TicTacToeCell> solver = solverFactory.Create();

            // Act
            IList<TicTacToeCell> forecast;
            int result = solver.Maximize(history, out forecast);

            // Assert
            Assert.AreEqual(int.MaxValue - 3, result);
            Assert.IsNotNull(forecast);
            Assert.AreEqual(3, forecast.Count);
            Assert.AreEqual(1, forecast[0].Row);
            Assert.AreEqual(1, forecast[0].Column);
        }

        [TestMethod]
        public void Solver_recognizes_lost_tic_tac_toe_position()
        {
            // Arrange

            // X . O
            // . . .
            // . . X
            List<TicTacToeCell> history = new List<TicTacToeCell>()
            {
                new TicTacToeCell(0, 0),
                new TicTacToeCell(0, 2),
                new TicTacToeCell(2, 2)
            };

            var externalLogics = new TicTacToeLogics();
            var solverConfiguration = GetSolverConfiguration();
            var solverFactory = new SolverFactory<TicTacToePosition, TicTacToeCell>(externalLogics, solverConfiguration);
            ISolver<TicTacToeCell> solver = solverFactory.Create();

            // Act
            IList<TicTacToeCell> forecast;
            int result = solver.Maximize(history, out forecast);

            // Assert
            Assert.AreEqual(-int.MaxValue + 4, result);
            Assert.IsNotNull(forecast);
            Assert.AreEqual(4, forecast.Count);
            Assert.AreEqual(1, forecast[0].Row);
            Assert.AreEqual(1, forecast[0].Column);
            Assert.AreEqual(2, forecast[1].Row);
            Assert.AreEqual(0, forecast[1].Column);
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void Solver_throws_exception_if_game_is_already_over()
        {
            // Arrange

            // X O X
            // O X O
            // X O X
            List<TicTacToeCell> history = new List<TicTacToeCell>()
            {
                new TicTacToeCell(0, 0),
                new TicTacToeCell(0, 1),
                new TicTacToeCell(0, 2),
                new TicTacToeCell(1, 1),
                new TicTacToeCell(1, 0),
                new TicTacToeCell(1, 2),
                new TicTacToeCell(2, 1),
                new TicTacToeCell(2, 0),
                new TicTacToeCell(2, 2)
            };

            var externalLogics = new TicTacToeLogics();
            var solverConfiguration = GetSolverConfiguration();
            var solverFactory = new SolverFactory<TicTacToePosition, TicTacToeCell>(externalLogics, solverConfiguration);
            ISolver<TicTacToeCell> solver = solverFactory.Create();

            // Act
            IList<TicTacToeCell> forecast;
            solver.Maximize(history, out forecast);
        }

        private ISolverConfiguration GetSolverConfiguration()
        {
            var solverConfiguration = new Mock<ISolverConfiguration>(MockBehavior.Strict);
            
            solverConfiguration
                .Setup(t => t.TimeLimitPerMove)
                .Returns(TimeSpan.FromSeconds(1));
            solverConfiguration
                .Setup(t => t.SearchDepthLimit)
                .Returns(10);
            solverConfiguration
                .Setup(t => t.QuiescenceExtensionLimit)
                .Returns(0);
            solverConfiguration
                .Setup(t => t.EvaluationTableSizeExponent)
                .Returns(8);
            solverConfiguration
                .Setup(t => t.EvaluationTableRetryLimit)
                .Returns(0);
            solverConfiguration
                .Setup(t => t.TranspositionTableSizeExponent)
                .Returns(20);
            solverConfiguration
                .Setup(t => t.TranspositionTableRetryLimit)
                .Returns(0);

            return solverConfiguration.Object;
        }
    }
}
