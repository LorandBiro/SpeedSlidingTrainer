using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ConsoleApplication1.BoardSolverV1;
using ConsoleApplication1.BoardSolverV2;
using ConsoleApplication1.BoardSolverV3;
using ConsoleApplication1.BoardSolverV4;
using ConsoleApplication1.BoardSolverV5;
using ConsoleApplication1.BoardSolverV6;
using SpeedSlidingTrainer.Core.Model.State;
using SpeedSlidingTrainer.Core.Services.BoardGenerator;

namespace ConsoleApplication1
{
    internal class Program
    {
        private static readonly IBoardGeneratorService Generator = new BoardGeneratorService(1337);

        private static void Main(string[] args)
        {
            Measure(
                "Simple",
                10,
                new IBoardSolver[] { new BoardSolverServiceV1(), new BoardSolverServiceV2(), new BoardSolverServiceV3(), new BoardSolverServiceV4(), new BoardSolverServiceV5(), new BoardSolverServiceV6() },
                BoardTemplate.CreateEmpty(4, 4),
                new BoardGoal(4, 4, new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));

            Measure(
                "Medium",
                10,
                new IBoardSolver[] { new BoardSolverServiceV2(), new BoardSolverServiceV3(), new BoardSolverServiceV4(), new BoardSolverServiceV5(), new BoardSolverServiceV6() },
                BoardTemplate.CreateEmpty(4, 4),
                new BoardGoal(4, 4, new[] { 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));

            Measure(
                "Hard",
                10,
                new IBoardSolver[] { new BoardSolverServiceV3(), new BoardSolverServiceV4(), new BoardSolverServiceV5(), new BoardSolverServiceV6() },
                BoardTemplate.CreateEmpty(4, 4),
                new BoardGoal(4, 4, new[] { 1, 2, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));

            Measure(
                "Harder",
                10,
                new IBoardSolver[] { new BoardSolverServiceV4(), new BoardSolverServiceV5(), new BoardSolverServiceV6() },
                BoardTemplate.CreateEmpty(4, 4),
                new BoardGoal(4, 4, new[] { 1, 2, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));

            Measure(
                "Extreme",
                10,
                new IBoardSolver[] { new BoardSolverServiceV4(), new BoardSolverServiceV5(), new BoardSolverServiceV6() },
                new BoardTemplate(4, 4, new[] { 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 4 }),
                new BoardGoal(4, 4, new[] { 1, 2, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));
        }

        private static void Measure(string name, int count, IBoardSolver[] solvers, BoardTemplate template, BoardGoal goal)
        {
            BoardState[] simpleBoards = Enumerable.Range(0, count).Select(x => Generator.Generate(template, goal)).ToArray();

            Measure(name, solvers, simpleBoards, goal);
        }

        private static void Measure(string name, IBoardSolver[] solvers, BoardState[] boards, BoardGoal goal)
        {
            foreach (IBoardSolver solver in solvers)
            {
                Measure(name + " " + solver.GetType().Name, () =>
                {
                    foreach (BoardState board in boards)
                    {
                        solver.GetSolution(board, goal, CancellationToken.None);
                        Console.Write(".");
                    }
                });
            }
        }

        private static void Measure(string name, Action action)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Console.Write(name);
            action();
            Console.WriteLine($": {sw.Elapsed}");
        }
    }
}
