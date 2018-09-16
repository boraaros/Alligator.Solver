using Alligator.Solver;
using Alligator.Solver.Demo;
using Alligator.TicTacToe;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Hello tic-tac-toe demo!");

            var rules = new TicTacToeRules();
            var solverConfiguration = new SolverConfiguration();
            var solverFactory = new SolverFactory<TicTacToePosition, TicTacToeCell>(rules, solverConfiguration, SolverLog);
            ISolver<TicTacToeCell> solver = solverFactory.Create();

            TicTacToePosition position = new TicTacToePosition();
            IList<TicTacToeCell> history = new List<TicTacToeCell>();
            bool aiStep = true;

            while (!position.IsEnded)
            {
                PrintPosition(position);
                TicTacToeCell next;
                TicTacToePosition copy = new TicTacToePosition(position.History);

                if (aiStep)
                {
                    while (true)
                    {
                        try
                        {
                            solver = solverFactory.Create();
                            next = AiStep(history, solver);
                            copy.Take(next);
                            break;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                else
                {
                    while (true)
                    {
                        try
                        {
                            next = HumanStep();
                            copy.Take(next);
                            break;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                position.Take(next);
                history.Add(next);
                aiStep = !aiStep;
            }
            if (!position.HasWinner)
            {
                Console.WriteLine("Game over, DRAW!");
            }
            else
            {
                Console.WriteLine(string.Format("Game over, {0} WON!", aiStep ? "human" : "ai"));
            }

            PrintPosition(position);

            Console.ReadKey();
        }

        private static TicTacToeCell HumanStep()
        {
            Console.Write("Next step [row:column]: ");
            while (true)
            {
                try
                {
                    string[] msg = Console.ReadLine().Split(':');
                    return new TicTacToeCell(int.Parse(msg[0]), int.Parse(msg[1]));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static TicTacToeCell AiStep(IList<TicTacToeCell> history, ISolver<TicTacToeCell> solver)
        {
            var next = solver.CalculateNextMove(history);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Alligator is thinking...");
            //Console.WriteLine(string.Format("Evaluation value: {0} ({1})", evaluationValue, ToString(evaluationValue)));
            Console.WriteLine(string.Format("Optimal next step: {0}", next));
            //Console.WriteLine(string.Format("Forecast: {0}", string.Join(" --> ", forecast)));
            Console.ForegroundColor = ConsoleColor.White;

            return next;
        }

        private static string ToString(int evaluationValue)
        {
            if (evaluationValue == 0)
            {
                return "Hm, draw..";
            }
            return evaluationValue > 0 ? "Ho-Ho-Ho!!!" : "Oh, no!";
        }

        private static void PrintPosition(TicTacToePosition position)
        {
            Console.WriteLine(string.Join("-", Enumerable.Range(0, TicTacToePosition.BoardSize + 1).Select(t => "-")));

            for (int i = 0; i < TicTacToePosition.BoardSize; i++)
            {
                for (int j = 0; j < TicTacToePosition.BoardSize; j++)
                {
                    switch (position.GetMarkAt(i, j))
                    {
                        case TicTacToeMark.Empty:
                            Console.Write(string.Format(" {0}", "."));
                            break;
                        case TicTacToeMark.X:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(string.Format(" {0}", TicTacToeMark.X));
                            break;
                        case TicTacToeMark.O:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write(string.Format(" {0}", TicTacToeMark.O));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException($"Unknown tic-tac-toe mark type: {position.GetMarkAt(i, j)}");
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine();
            }
            Console.WriteLine(string.Join("-", Enumerable.Range(0, TicTacToePosition.BoardSize + 1).Select(t => "-")));
        }

        private static void SolverLog(string message)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(string.Format("[SolverLog] {0}", message));
            Console.ForegroundColor = prevColor;
        }
    }
}
