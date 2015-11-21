using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Core.Model.State.Validation;

namespace SpeedSlidingTrainer.Core.Model.State
{
    public abstract class BoardStateBase : IEquatable<BoardStateBase>
    {
        [NotNull]
        private readonly int[] values;

        protected BoardStateBase(int width, int height, [NotNull] int[] values, ValidationType validation)
        {
            BoardValidationError[] errors;
            if (!Validate(width, height, values, validation, out errors))
            {
                throw new InvalidBoardException(errors);
            }

            this.values = (int[])values.Clone();
            this.Width = width;
            this.Height = height;
        }

        protected BoardStateBase(int width, int height, [NotNull] int[] values)
        {
            this.Width = width;
            this.Height = height;
            this.values = values;
        }

        protected enum ValidationType
        {
            BoardState,
            BoardTemplate,
            BoardGoal,
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int TileCount
        {
            get
            {
                return this.values.Length;
            }
        }

        public int this[int index]
        {
            get
            {
                return this.values[index];
            }
        }

        public int this[int x, int y]
        {
            get
            {
                return this.values[(y * this.Width) + x];
            }
        }

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

        [NotNull]
        public int[] GetValues()
        {
            return (int[])this.values.Clone();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null) || this.GetType() != obj.GetType())
            {
                return false;
            }

            BoardStateBase other = (BoardStateBase)obj;
            if (this.Width != other.Width || this.Height != other.Height)
            {
                return false;
            }

            for (int i = 0; i < this.TileCount; i++)
            {
                if (this[i] != other[i])
                {
                    return false;
                }
            }

            return true;
        }

        public bool Equals(BoardStateBase other)
        {
            return this.Equals((object)other);
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            for (int i = 0; i < this.TileCount; i++)
            {
                hashCode = (hashCode * 397) ^ this[i];
            }

            return hashCode;
        }

        [NotNull]
        public override string ToString()
        {
            int maxValue = this.values.Length - 1;
            int padding = maxValue.ToString().Length + 1;

            StringBuilder sb = new StringBuilder();
            int index = 0;
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    int value = this.values[index++];
                    string str = value == 0 ? string.Empty : value.ToString();
                    sb.Append(str.PadLeft(padding));
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        protected static bool Validate(int width, int height, [NotNull] int[] values, ValidationType validationType, [NotNull] out BoardValidationError[] errors)
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
                            string.Format("For a {0}x{1} board size the biggest allowed value is {2}.", width, height, maxValue),
                            BoardPosition.FromIndex(width, height, i)));
                }

                for (int j = i + 1; j < values.Length; j++)
                {
                    if (values[j] == values[i])
                    {
                        problems.Add(
                            new BoardValidationError(
                                BoardValidationErrorType.Duplication,
                                string.Format("The value {0} can be found multiple times on the board.", values[j]),
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
                    else if (unspecifiedCount < 3)
                    {
                        if (problems.Count == 0 && !IsSolvable(width, height, values))
                        {
                            problems.Add(new BoardValidationError(BoardValidationErrorType.NotSolvable, "The board is not solvable."));
                        }
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
