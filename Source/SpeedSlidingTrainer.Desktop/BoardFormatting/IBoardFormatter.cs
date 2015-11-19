using System;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Core.Model.State;

namespace SpeedSlidingTrainer.Desktop.BoardFormatting
{
    public interface IBoardFormatter
    {
        /// <exception cref="ArgumentNullException">If <see cref="boardState"/> is <c>null</c>.</exception>
        [NotNull]
        string ToString([NotNull] BoardStateBase boardState);

        /// <exception cref="ArgumentNullException">If <see cref="boardDescription"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidBoardDescriptionException">If the format of <see cref="boardDescription"/> is invalid and couldn't be parsed.</exception>
        [NotNull]
        BoardDescription ParseBoardDescription([NotNull] string boardDescription);
    }
}
