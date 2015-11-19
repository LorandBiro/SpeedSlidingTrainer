using JetBrains.Annotations;
using SpeedSlidingTrainer.Core.Model.State;

namespace SpeedSlidingTrainer.Core.Services.BoardGenerator
{
    public interface IBoardGeneratorService
    {
        [NotNull]
        BoardState Generate([NotNull] BoardTemplate template);

        [NotNull]
        BoardState Generate([NotNull] BoardTemplate template, [NotNull] BoardGoal goal);
    }
}
