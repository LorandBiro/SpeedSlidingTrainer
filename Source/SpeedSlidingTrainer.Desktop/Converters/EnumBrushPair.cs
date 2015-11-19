using System;
using System.Windows.Media;
using JetBrains.Annotations;

namespace SpeedSlidingTrainer.Desktop.Converters
{
    public sealed class EnumBrushPair
    {
        [CanBeNull]
        public Enum Key { get; set; }

        [CanBeNull]
        public Brush Brush { get; set; }
    }
}
