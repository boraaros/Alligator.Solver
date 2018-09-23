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

            var rules = new TicTacToeRules();
            var solverConfiguration = GetSolverConfiguration();
            var solverFactory = new SolverFactory<TicTacToePosition, TicTacToeCell>(rules, solverConfiguration);
            ISolver<TicTacToeCell> solver = solverFactory.Create();

            // Act
            TicTacToeCell nextPly = solver.OptimizeNextMove(history);

            // Assert
            Assert.IsNotNull(nextPly);
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

            var rules = new TicTacToeRules();
            var solverConfiguration = GetSolverConfiguration();
            var solverFactory = new SolverFactory<TicTacToePosition, TicTacToeCell>(rules, solverConfiguration);
            ISolver<TicTacToeCell> solver = solverFactory.Create();

            // Act
            TicTacToeCell nextPly = solver.OptimizeNextMove(history);

            // Assert
            Assert.AreEqual(1, nextPly.Row);
            Assert.AreEqual(1, nextPly.Column);
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

            var rules = new TicTacToeRules();
            var solverConfiguration = GetSolverConfiguration();
            var solverFactory = new SolverFactory<TicTacToePosition, TicTacToeCell>(rules, solverConfiguration);
            ISolver<TicTacToeCell> solver = solverFactory.Create();

            // Act
            TicTacToeCell nextPly = solver.OptimizeNextMove(history);

            // Assert
            Assert.AreEqual(1, nextPly.Row);
            Assert.AreEqual(1, nextPly.Column);
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

            var rules = new TicTacToeRules();
            var solverConfiguration = GetSolverConfiguration();
            var solverFactory = new SolverFactory<TicTacToePosition, TicTacToeCell>(rules, solverConfiguration);
            ISolver<TicTacToeCell> solver = solverFactory.Create();

            // Act
            TicTacToeCell nextPly = solver.OptimizeNextMove(history);
        }

        private ISolverConfiguration GetSolverConfiguration()
        {
            var solverConfiguration = new Mock<ISolverConfiguration>(MockBehavior.Strict);
            
            solverConfiguration
                .Setup(t => t.TimeLimitPerMove)
                .Returns(TimeSpan.FromSeconds(1));

            return solverConfiguration.Object;
        }
    }
}
