using System.Collections.Generic;
using System.Threading;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;

namespace ConsoleApplication1
{
    public interface IBoardSolver
    {
        List<Step[]> GetSolution(BoardState state, BoardGoal goal, CancellationToken cancellationToken);
    }
}
