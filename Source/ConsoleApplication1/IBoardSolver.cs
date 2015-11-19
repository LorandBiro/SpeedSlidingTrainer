namespace ConsoleApplication1
{
    using System.Threading;
    using SpeedSlidingTrainer.Core.Model;
    using SpeedSlidingTrainer.Core.Model.State;

    public interface IBoardSolver
    {
        Step[] GetSolution(BoardState state, BoardGoal goal, CancellationToken cancellationToken);
    }
}
