using System.Threading;
using SpeedSlidingTrainer.Core.Model.State;
using SpeedSlidingTrainer.Core.Services.BoardGenerator;
using SpeedSlidingTrainer.Core.Services.BoardSolver;

namespace ConsoleApplication1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IBoardGeneratorService generator = new BoardGeneratorService();
            BoardState initialState = generator.Generate(new BoardTemplate(4, 4, new[] { 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 4 }));
            BoardGoal goal = new BoardGoal(4, 4, new[] { 1, 2, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

            IBoardSolverService solver = new BoardSolverService();
            solver.GetSolution(initialState, goal, CancellationToken.None);
        }
    }
}
