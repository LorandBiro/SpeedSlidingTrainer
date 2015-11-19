using System.Threading;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;

namespace SpeedSlidingTrainer.Core.Services.BoardSolver
{
    public interface IBoardSolverService
    {
        [NotNull]
        Step[] GetSolution([NotNull] BoardState initialState, [NotNull] BoardGoal goal, CancellationToken cancellationToken);
    }
}
