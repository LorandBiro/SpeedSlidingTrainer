using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SpeedSlidingTrainer.Core.Model.State.Validation
{
    public static class BoardValidator
    {
        public static bool IsSolvable(int width, int height, int[] values)
        {
            ThrowIfInvalid(width, height, values);

            int inversionCount = 0;
            int eyeY = 0;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == 0)
                {
                    eyeY = i / width;
                }

                for (int j = i + 1; j < values.Length; j++)
                {
                    if (values[j] != 0 && values[j] < values[i])
                    {
                        inversionCount++;
                    }
                }
            }

            bool widthIsEven = (width % 2) == 0;
            bool inversionCountIsEven = (inversionCount % 2) == 0;
            if (widthIsEven)
            {
                bool eyeIsOnEvenRowFromBottom = ((height - (eyeY + 1)) % 2) == 0;
                return eyeIsOnEvenRowFromBottom == inversionCountIsEven;
            }

            return inversionCountIsEven;
        }

        public static bool Validate(int width, int height, [NotNull] int[] values, ValidationType validationType, [NotNull] out BoardValidationError[] errors)
        {
            ThrowIfInvalid(width, height, values);

            List<BoardValidationError> problems = new List<BoardValidationError>();
            int maxValue = values.Length - 1;
            int unspecifiedCount = 0;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == 0)
                {
                    unspecifiedCount++;
                    continue;
                }

                if (values[i] < 0)
                {
                    problems.Add(new BoardValidationError(BoardValidationErrorType.ValueOutOfRange, "Negative values are invalid.", BoardPosition.FromIndex(width, height, i)));
                }
                else if (values[i] > maxValue)
                {
                    problems.Add(
                        new BoardValidationError(
                            BoardValidationErrorType.ValueOutOfRange,
                            $"For a {width}x{height} board size the biggest allowed value is {maxValue}.",
                            BoardPosition.FromIndex(width, height, i)));
                }

                for (int j = i + 1; j < values.Length; j++)
                {
                    if (values[j] == values[i])
                    {
                        problems.Add(
                            new BoardValidationError(
                                BoardValidationErrorType.Duplication,
                                $"The value {values[j]} can be found multiple times on the board.",
                                BoardPosition.FromIndex(width, height, j)));
                    }
                }
            }

            // At this point: Values are all in range, there are no duplications, unspecifiedCount is at least 1
            switch (validationType)
            {
                case ValidationType.BoardState:
                    if (unspecifiedCount > 1)
                    {
                        problems.Add(new BoardValidationError(BoardValidationErrorType.Duplication, "Empty slot can be found multiple times on the board."));
                        break;
                    }

                    if (problems.Count == 0 && !IsSolvable(width, height, values))
                    {
                        problems.Add(new BoardValidationError(BoardValidationErrorType.NotSolvable, "The board is not solvable."));
                    }

                    break;

                case ValidationType.BoardTemplate:
                    if (unspecifiedCount < 3)
                    {
                        problems.Add(new BoardValidationError(BoardValidationErrorType.NotEnoughUnspecifiedTiles, "There must be at least 3 unspecified tiles."));
                    }

                    break;

                case ValidationType.BoardGoal:
                    if (unspecifiedCount == values.Length)
                    {
                        problems.Add(new BoardValidationError(BoardValidationErrorType.NotSolvable, "There are no specified tiles."));
                    }
                    else if (unspecifiedCount == 1)
                    {
                        if (problems.Count == 0 && !IsSolvable(width, height, values))
                        {
                            problems.Add(new BoardValidationError(BoardValidationErrorType.NotSolvable, "The board is not solvable."));
                        }
                    }
                    else if (unspecifiedCount == 2)
                    {
                        problems.Add(new BoardValidationError(BoardValidationErrorType.NotSolvable, "Board goals with 2 unspecified tile is not supported."));
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(validationType));
            }

            errors = problems.ToArray();
            return problems.Count == 0;
        }

        private static void ThrowIfInvalid(int width, int height, [NotNull] int[] values)
        {
            if (width < 2)
            {
                throw new ArgumentException("Width must be at least 2.", nameof(width));
            }

            if (height < 2)
            {
                throw new ArgumentException("Height must be at least 2.", nameof(height));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (width * height != values.Length)
            {
                throw new ArgumentException("Not enough values specified for this size.", nameof(values));
            }
        }
    }
}
