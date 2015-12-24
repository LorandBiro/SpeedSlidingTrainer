using System;
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
            if (!BoardValidator.Validate(width, height, values, validation, out errors))
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

        public int Width { get; }

        public int Height { get; }

        public int TileCount => this.values.Length;

        public int this[int index] => this.values[index];

        public int this[int x, int y] => this.values[(y * this.Width) + x];

        [NotNull]
        public int[] GetValues()
        {
            return (int[])this.values.Clone();
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as BoardStateBase);
        }

        public bool Equals(BoardStateBase other)
        {
            if (other == null || other.GetType() != this.GetType())
            {
                return false;
            }

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
    }
}
