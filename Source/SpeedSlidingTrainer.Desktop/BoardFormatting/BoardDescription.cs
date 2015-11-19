using System;
using JetBrains.Annotations;

namespace SpeedSlidingTrainer.Desktop.BoardFormatting
{
    public sealed class BoardDescription
    {
        public BoardDescription(int width, int height, [NotNull] int[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (width * height != values.Length)
            {
                throw new ArgumentException("The length of the values doesn't match the specified dimensions.", nameof(values));
            }

            this.Width = width;
            this.Height = height;
            this.Values = values;
        }

        public int Width { get; }

        public int Height { get; }

        [NotNull]
        public int[] Values { get; }
    }
}
