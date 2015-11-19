namespace SpeedSlidingTrainer.Core.Services
{
    using System;
    using JetBrains.Annotations;
    using SpeedSlidingTrainer.Core.Model.State;

    public interface IBoardGeneratorService
    {
        [NotNull]
        BoardState Generate([NotNull] BoardTemplate template);
    }
}
