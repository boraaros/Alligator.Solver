//using Alligator.Solver.Algorithm;
//using Alligator.Solver.Caches;
//using Alligator.Solver.Heuristics;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
//using System;
//using System.Linq;
//using System.Collections.Generic;

//namespace Alligator.Solver.UnitTests.Algorithm
//{
//    [TestClass]
//    public class NegaScoutTests
//    {
//        [TestMethod]
//        public void Negascout_does_not_call_static_evaluation_for_intermediate_nodes()
//        {
//            // Arrange
//            var staticEvaluatedIds = new List<ulong>();

//            var cacheTables = new Mock<ICacheTables<TestPosition, TesTMove>>(MockBehavior.Loose);

//            var heuristicTables = new Mock<IHeuristicTables<TesTMove>>(MockBehavior.Loose);

//            var externalLogics = new Mock<IExternalLogics<TestPosition, TesTMove>>(MockBehavior.Strict);
//            externalLogics
//                .Setup(t => t.CreateEmptyPosition())
//                .Returns(new TestPosition((id) => false, (id) => false, (id) => true));
//            externalLogics
//                .Setup(t => t.GetStrategiesFrom(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>((p) => new List<TesTMove>
//                    {
//                        new TesTMove(p.Identifier + 1),
//                        new TesTMove(p.Identifier + 2)
//                    });
//            externalLogics
//                .Setup(t => t.StaticEvaluate(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>(p => (int)p.Identifier)
//                .Callback<TestPosition>(p => staticEvaluatedIds.Add(p.Identifier));

//            var miniMaxSettings = new MiniMaxSettings(2, 0);

//            var negaScout = new NegaScout<TestPosition, TesTMove>(
//                externalLogics.Object, cacheTables.Object, heuristicTables.Object, miniMaxSettings);

//            var position = new TestPosition((id) => false, (id) => false, (id) => true);

//            // Act
//            var result = negaScout.Start(position);

//            // Assert
//            Assert.IsTrue(staticEvaluatedIds.All(t => t > 2));
//        }

//        [TestMethod]
//        public void Negascout_calls_static_evaluation_for_all_leaf_nodes_without_cut_off()
//        {
//            // Arrange
//            var staticEvaluatedIds = new List<ulong>();

//            var cacheTables = new Mock<ICacheTables<TestPosition, TesTMove>>(MockBehavior.Loose);

//            var heuristicTables = new Mock<IHeuristicTables<TesTMove>>(MockBehavior.Loose);

//            var externalLogics = new Mock<IExternalLogics<TestPosition, TesTMove>>(MockBehavior.Strict);
//            externalLogics
//                .Setup(t => t.CreateEmptyPosition())
//                .Returns(new TestPosition((id) => false, (id) => false, (id) => true));
//            externalLogics
//                .Setup(t => t.GetStrategiesFrom(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>((p) => new List<TesTMove>
//                    {
//                        new TesTMove(p.Identifier + 1),
//                        new TesTMove(p.Identifier + 2)
//                    });
//            externalLogics
//                .Setup(t => t.StaticEvaluate(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>(p => (int)p.Identifier)
//                .Callback<TestPosition>(p => staticEvaluatedIds.Add(p.Identifier));

//            var miniMaxSettings = new MiniMaxSettings(2, 0);

//            var negaScout = new NegaScout<TestPosition, TesTMove>(
//                externalLogics.Object, cacheTables.Object, heuristicTables.Object, miniMaxSettings);

//            var position = new TestPosition((id) => false, (id) => false, (id) => true);

//            // Act
//            var result = negaScout.Start(position);

//            // Assert
//            Assert.IsTrue(staticEvaluatedIds.Contains(3ul));
//            Assert.IsTrue(staticEvaluatedIds.Contains(4ul));
//            Assert.IsTrue(staticEvaluatedIds.Contains(5ul));
//            Assert.IsTrue(staticEvaluatedIds.Contains(6ul));
//        }

//        [TestMethod]
//        public void Negascout_finds_the_optimum_if_opponent_moves_finally()
//        {
//            // Arrange
//            var cacheTables = new Mock<ICacheTables<TestPosition, TesTMove>>(MockBehavior.Loose);

//            var heuristicTables = new Mock<IHeuristicTables<TesTMove>>(MockBehavior.Loose);

//            var externalLogics = new Mock<IExternalLogics<TestPosition, TesTMove>>(MockBehavior.Strict);
//            externalLogics
//                .Setup(t => t.CreateEmptyPosition())
//                .Returns(new TestPosition((id) => false, (id) => false, (id) => true));
//            externalLogics
//                .Setup(t => t.GetStrategiesFrom(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>((p) => new List<TesTMove>
//                    {
//                        new TesTMove(p.Identifier + 1),
//                        new TesTMove(p.Identifier + 2)
//                    });
//            externalLogics
//                .Setup(t => t.StaticEvaluate(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>(p => (int)p.Identifier);

//            var miniMaxSettings = new MiniMaxSettings(2, 0);

//            var negaScout = new NegaScout<TestPosition, TesTMove>(
//                externalLogics.Object, cacheTables.Object, heuristicTables.Object, miniMaxSettings);

//            var position = new TestPosition((id) => false, (id) => false, (id) => true);

//            // Act
//            var result = negaScout.Start(position);

//            // Assert
//            Assert.AreEqual(5, result);
//        }

//        [TestMethod]
//        public void Negascout_finds_the_optimum_if_itself_moves_finally()
//        {
//            // Arrange
//            var cacheTables = new Mock<ICacheTables<TestPosition, TesTMove>>(MockBehavior.Loose);

//            var heuristicTables = new Mock<IHeuristicTables<TesTMove>>(MockBehavior.Loose);

//            var externalLogics = new Mock<IExternalLogics<TestPosition, TesTMove>>(MockBehavior.Strict);
//            externalLogics
//                .Setup(t => t.CreateEmptyPosition())
//                .Returns(new TestPosition((id) => false, (id) => false, (id) => true));
//            externalLogics
//                .Setup(t => t.GetStrategiesFrom(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>((p) => new List<TesTMove>
//                    {
//                        new TesTMove(p.Identifier + 1),
//                        new TesTMove(p.Identifier + 2)
//                    });
//            externalLogics
//                .Setup(t => t.StaticEvaluate(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>(p => (int)p.Identifier);

//            var miniMaxSettings = new MiniMaxSettings(3, 0);

//            var negaScout = new NegaScout<TestPosition, TesTMove>(
//                externalLogics.Object, cacheTables.Object, heuristicTables.Object, miniMaxSettings);

//            var position = new TestPosition((id) => false, (id) => false, (id) => true);

//            // Act
//            var result = negaScout.Start(position);

//            // Assert
//            Assert.AreEqual(12, result);
//        }

//        [TestMethod]
//        public void Negascout_recognizes_winnable_positions()
//        {
//            // Arrange
//            var winIds = new List<ulong> { 12ul, 13ul };

//            var cacheTables = new Mock<ICacheTables<TestPosition, TesTMove>>(MockBehavior.Loose);

//            var heuristicTables = new Mock<IHeuristicTables<TesTMove>>(MockBehavior.Loose);

//            var externalLogics = new Mock<IExternalLogics<TestPosition, TesTMove>>(MockBehavior.Strict);
//            externalLogics
//                .Setup(t => t.CreateEmptyPosition())
//                .Returns(new TestPosition((id) => winIds.Contains(id), (id) => winIds.Contains(id), (id) => true));
//            externalLogics
//                .Setup(t => t.GetStrategiesFrom(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>((p) => new List<TesTMove>
//                    {
//                        new TesTMove(p.Identifier + 1),
//                        new TesTMove(p.Identifier + 2)
//                    });
//            externalLogics
//                .Setup(t => t.StaticEvaluate(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>(p => (int)p.Identifier);

//            var miniMaxSettings = new MiniMaxSettings(3, 0);

//            var negaScout = new NegaScout<TestPosition, TesTMove>(
//                externalLogics.Object, cacheTables.Object, heuristicTables.Object, miniMaxSettings);

//            var position = new TestPosition((id) => winIds.Contains(id), (id) => winIds.Contains(id), (id) => true);

//            // Act
//            var result = negaScout.Start(position);

//            // Assert
//            Assert.AreEqual(int.MaxValue - miniMaxSettings.MaxDepth, result);
//        }

//        [TestMethod]
//        public void Negascout_recognizes_losing_positions()
//        {
//            // Arrange
//            var winIds = new List<ulong> { 4ul, 5ul };

//            var cacheTables = new Mock<ICacheTables<TestPosition, TesTMove>>(MockBehavior.Loose);

//            var heuristicTables = new Mock<IHeuristicTables<TesTMove>>(MockBehavior.Loose);

//            var externalLogics = new Mock<IExternalLogics<TestPosition, TesTMove>>(MockBehavior.Strict);
//            externalLogics
//                .Setup(t => t.CreateEmptyPosition())
//                .Returns(new TestPosition((id) => winIds.Contains(id), (id) => winIds.Contains(id), (id) => true));
//            externalLogics
//                .Setup(t => t.GetStrategiesFrom(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>((p) => new List<TesTMove>
//                    {
//                        new TesTMove(p.Identifier + 1),
//                        new TesTMove(p.Identifier + 2)
//                    });
//            externalLogics
//                .Setup(t => t.StaticEvaluate(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>(p => (int)p.Identifier);

//            var miniMaxSettings = new MiniMaxSettings(2, 0);

//            var negaScout = new NegaScout<TestPosition, TesTMove>(
//                externalLogics.Object, cacheTables.Object, heuristicTables.Object, miniMaxSettings);

//            var position = new TestPosition((id) => winIds.Contains(id), (id) => winIds.Contains(id), (id) => true);

//            // Act
//            var result = negaScout.Start(position);

//            // Assert
//            Assert.AreEqual(-int.MaxValue + miniMaxSettings.MaxDepth, result);
//        }

//        [TestMethod]
//        public void Negascout_finds_beta_cutoff_and_prunes_the_search_tree()
//        {
//            // Arrange
//            var cacheTables = new Mock<ICacheTables<TestPosition, TesTMove>>(MockBehavior.Loose);

//            var heuristicTables = new Mock<IHeuristicTables<TesTMove>>(MockBehavior.Loose);

//            var externalLogics = new Mock<IExternalLogics<TestPosition, TesTMove>>(MockBehavior.Strict);
//            externalLogics
//                .Setup(t => t.CreateEmptyPosition())
//                .Returns(new TestPosition((id) => false, (id) => false, (id) => true));
//            externalLogics
//                .Setup(t => t.GetStrategiesFrom(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>((p) => new List<TesTMove>
//                    {
//                        new TesTMove(p.Identifier + 1),
//                        new TesTMove(p.Identifier + 2)
//                    });
//            externalLogics
//                .Setup(t => t.StaticEvaluate(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>(p => 6 - (int)p.Identifier);

//            var miniMaxSettings = new MiniMaxSettings(2, 0);

//            var negaScout = new NegaScout<TestPosition, TesTMove>(
//                externalLogics.Object, cacheTables.Object, heuristicTables.Object, miniMaxSettings);

//            var position = new TestPosition((id) => false, (id) => false, (id) => true);

//            // Act
//            var result = negaScout.Start(position);

//            // Assert
//            Assert.AreEqual(2, result);
//            externalLogics.Verify(t => t.StaticEvaluate(It.Is<TestPosition>(p => p.Identifier == 6ul)), Times.Never);
//        }

//        [TestMethod]
//        public void Negascout_means_beta_cutoff_to_heuristic_tables()
//        {
//            // Arrange
//            var cacheTables = new Mock<ICacheTables<TestPosition, TesTMove>>(MockBehavior.Loose);

//            var heuristicTables = new Mock<IHeuristicTables<TesTMove>>(MockBehavior.Loose);

//            var externalLogics = new Mock<IExternalLogics<TestPosition, TesTMove>>(MockBehavior.Strict);
//            externalLogics
//                .Setup(t => t.CreateEmptyPosition())
//                .Returns(new TestPosition((id) => false, (id) => false, (id) => true));
//            externalLogics
//                .Setup(t => t.GetStrategiesFrom(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>((p) => new List<TesTMove>
//                    {
//                        new TesTMove(p.Identifier + 1),
//                        new TesTMove(p.Identifier + 2)
//                    });
//            externalLogics
//                .Setup(t => t.StaticEvaluate(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>(p => 6 - (int)p.Identifier);

//            var miniMaxSettings = new MiniMaxSettings(2, 0);

//            var negaScout = new NegaScout<TestPosition, TesTMove>(
//                externalLogics.Object, cacheTables.Object, heuristicTables.Object, miniMaxSettings);

//            var position = new TestPosition((id) => false, (id) => false, (id) => true);

//            // Act
//            var result = negaScout.Start(position);

//            // Assert
//            heuristicTables.Verify(t => t.StoreBetaCutOff(It.Is<TesTMove>(p => p.Value == 3ul), 1), Times.Once);
//        }

//        [TestMethod]
//        public void Negascout_add_transpositions_to_cache()
//        {
//            // Arrange
//            var idsToCache = new List<ulong>();

//            var cacheTables = new Mock<ICacheTables<TestPosition, TesTMove>>(MockBehavior.Loose);
//            cacheTables
//                .Setup(t => t.AddTransposition(It.IsAny<TestPosition>(), It.IsAny<Transposition<TesTMove>>()))
//                .Callback<TestPosition, Transposition<TesTMove>>((p, t) => idsToCache.Add(p.Identifier));

//            var heuristicTables = new Mock<IHeuristicTables<TesTMove>>(MockBehavior.Loose);

//            var externalLogics = new Mock<IExternalLogics<TestPosition, TesTMove>>(MockBehavior.Strict);
//            externalLogics
//                .Setup(t => t.CreateEmptyPosition())
//                .Returns(new TestPosition((id) => false, (id) => false, (id) => true));
//            externalLogics
//                .Setup(t => t.GetStrategiesFrom(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>((p) => new List<TesTMove>
//                    {
//                        new TesTMove(p.Identifier + 1),
//                        new TesTMove(p.Identifier + 2)
//                    });
//            externalLogics
//                .Setup(t => t.StaticEvaluate(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>(p => 6 - (int)p.Identifier);

//            var miniMaxSettings = new MiniMaxSettings(2, 0);

//            var negaScout = new NegaScout<TestPosition, TesTMove>(
//                externalLogics.Object, cacheTables.Object, heuristicTables.Object, miniMaxSettings);

//            var position = new TestPosition((id) => false, (id) => false, (id) => true);

//            // Act
//            var result = negaScout.Start(position);

//            // Assert
//            Assert.AreEqual(3, idsToCache.Count);
//            Assert.IsTrue(idsToCache.Contains(0ul));
//            Assert.IsTrue(idsToCache.Contains(1ul));
//            Assert.IsTrue(idsToCache.Contains(2ul));
//        }

//        [TestMethod]
//        public void Negascout_quiescence_extension_called_if_leaf_is_not_quiet()
//        {
//            // Arrange
//            var quietIds = new List<ulong> { 3ul, 5ul, 6ul };
//            var expandedIds = new List<ulong>();

//            var cacheTables = new Mock<ICacheTables<TestPosition, TesTMove>>(MockBehavior.Loose);

//            var heuristicTables = new Mock<IHeuristicTables<TesTMove>>(MockBehavior.Loose);

//            var externalLogics = new Mock<IExternalLogics<TestPosition, TesTMove>>(MockBehavior.Strict);
//            externalLogics
//                .Setup(t => t.CreateEmptyPosition())
//                .Returns(new TestPosition((id) => false, (id) => false, (id) => quietIds.Contains(id)));
//            externalLogics
//                .Setup(t => t.GetStrategiesFrom(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>((p) => new List<TesTMove>
//                    {
//                        new TesTMove(p.Identifier + 1),
//                        new TesTMove(p.Identifier + 2)
//                    })
//                .Callback<TestPosition>(p => expandedIds.Add(p.Identifier));
//            externalLogics
//                .Setup(t => t.StaticEvaluate(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>(p => 6 - (int)p.Identifier);

//            var miniMaxSettings = new MiniMaxSettings(2, 1);

//            var negaScout = new NegaScout<TestPosition, TesTMove>(
//                externalLogics.Object, cacheTables.Object, heuristicTables.Object, miniMaxSettings);

//            var position = new TestPosition((id) => false, (id) => false, (id) => quietIds.Contains(id));

//            // Act
//            var result = negaScout.Start(position);

//            // Assert
//            Assert.IsFalse(expandedIds.Contains(3ul));
//            Assert.IsTrue(expandedIds.Contains(4ul));
//            Assert.IsFalse(expandedIds.Contains(5ul));
//            Assert.IsFalse(expandedIds.Contains(6ul));
//            Assert.IsFalse(expandedIds.Contains(9ul));
//            Assert.IsFalse(expandedIds.Contains(10ul));
//        }

//        [TestMethod]
//        public void Negascout_does_not_store_transpositions_during_quiescence_extension()
//        {
//            // Arrange
//            var idsToCache = new List<ulong>();

//            var cacheTables = new Mock<ICacheTables<TestPosition, TesTMove>>(MockBehavior.Loose);
//            cacheTables
//                .Setup(t => t.AddTransposition(It.IsAny<TestPosition>(), It.IsAny<Transposition<TesTMove>>()))
//                .Callback<TestPosition, Transposition<TesTMove>>((p, t) => idsToCache.Add(p.Identifier));

//            var heuristicTables = new Mock<IHeuristicTables<TesTMove>>(MockBehavior.Loose);

//            var externalLogics = new Mock<IExternalLogics<TestPosition, TesTMove>>(MockBehavior.Strict);
//            externalLogics
//                .Setup(t => t.CreateEmptyPosition())
//                .Returns(new TestPosition((id) => false, (id) => false, (id) => false));
//            externalLogics
//                .Setup(t => t.GetStrategiesFrom(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>((p) => new List<TesTMove>
//                    {
//                        new TesTMove(p.Identifier + 1),
//                        new TesTMove(p.Identifier + 2)
//                    });
//            externalLogics
//                .Setup(t => t.StaticEvaluate(It.IsAny<TestPosition>()))
//                .Returns<TestPosition>(p => 12 - (int)p.Identifier);

//            var miniMaxSettings = new MiniMaxSettings(2, 2);

//            var negaScout = new NegaScout<TestPosition, TesTMove>(
//                externalLogics.Object, cacheTables.Object, heuristicTables.Object, miniMaxSettings);

//            var position = new TestPosition((id) => false, (id) => false, (id) => false);

//            // Act
//            var result = negaScout.Start(position);

//            // Assert
//            Assert.IsTrue(idsToCache.All(id => id <= 6));
//        }
//    }
//}