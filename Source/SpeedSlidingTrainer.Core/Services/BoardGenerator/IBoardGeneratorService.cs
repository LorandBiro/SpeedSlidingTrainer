using JetBrains.Annotations;
using SpeedSlidingTrainer.Core.Model.State;

namespace SpeedSlidingTrainer.Core.Services
{
    public interface IBoardGeneratorService
    {
        [NotNull]
        BoardState Generate([NotNull] BoardTemplate template);
    }
}
