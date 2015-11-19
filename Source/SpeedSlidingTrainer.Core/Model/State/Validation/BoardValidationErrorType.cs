namespace SpeedSlidingTrainer.Core.Model.State.Validation
{
    using System;

    public enum BoardValidationErrorType
    {
        ValueOutOfRange,
        Duplication,
        NotEnoughUnspecifiedTiles,
        NotSolvable,
    }
}
