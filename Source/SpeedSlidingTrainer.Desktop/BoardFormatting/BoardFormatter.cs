using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpeedSlidingTrainer.Core.Model.State;

namespace SpeedSlidingTrainer.Desktop.BoardFormatting
{
    public sealed class BoardFormatter : IBoardFormatter
    {
        public string ToString(BoardStateBase boardState)
        {
            if (boardState == null)
            {
                throw new ArgumentNullException(nameof(boardState));
            }

            int maxValue = boardState.TileCount - 1;
            int padding = maxValue.ToString().Length + 1;

            StringBuilder sb = new StringBuilder();
            int index = 0;
            for (int y = 0; y < boardState.Height; y++)
            {
                for (int x = 0; x < boardState.Width; x++)
                {
                    int value = boardState[index++];
                    string str = value == 0 ? "x" : value.ToString();
                    sb.Append(str.PadLeft(padding));
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public BoardDescription ParseBoardDescription(string boardDescription)
        {
            if (boardDescription == null)
            {
                throw new ArgumentNullException(nameof(boardDescription));
            }

            List<List<string>> lines =
                boardDescription.Split("\r\n".ToCharArray())
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(line => line.Split().Where(word => word != string.Empty).ToList())
                    .ToList();

            int height = lines.Count;
            if (height < 2)
            {
                throw new InvalidBoardDescriptionException("The board must be at least 2x2.");
            }

            int width = lines[0].Count;
            if (width < 2)
            {
                throw new InvalidBoardDescriptionException("The board must be at least 2x2.");
            }

            if (lines.Any(line => line.Count != width))
            {
                throw new InvalidBoardDescriptionException("Every row must have the same number of tiles.");
            }

            int[] values = new int[width * height];
            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    string word = lines[y][x];
                    if (word.Equals("x", StringComparison.OrdinalIgnoreCase))
                    {
                        values[index++] = 0;
                        continue;
                    }

                    int value;
                    if (int.TryParse(word, out value))
                    {
                        values[index++] = value;
                        continue;
                    }

                    throw new InvalidBoardDescriptionException($"'{word}' is an invalid value.");
                }
            }

            return new BoardDescription(width, height, values);
        }
    }
}