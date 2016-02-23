using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Core.Model.State;
using SpeedSlidingTrainer.Core.Model.State.Validation;

namespace SpeedSlidingTrainer.Core.Services.BoardGenerator
{
    public sealed class BoardGeneratorService : IBoardGeneratorService
    {
        private const int RetryCount = 100;

        private readonly Random random;

        public BoardGeneratorService(int? seed = null)
        {
            this.random = seed == null ? new Random() : new Random(seed.Value);
        }

        public BoardState Generate(BoardTemplate template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            int[] values = template.GetValues();
            List<int> set = GetUnspecifiedValues(values);
            List<int> unspecifiedIndices = GetUnspecifiedIndices(values);

            foreach (int unspecifiedIndex in unspecifiedIndices)
            {
                int i = this.random.Next(0, set.Count);
                values[unspecifiedIndex] = set[i];
                set.RemoveAt(i);
            }

            if (!BoardValidator.IsSolvable(template.Width, template.Height, values))
            {
                int i = unspecifiedIndices[0];
                int j = unspecifiedIndices[1];
                if (values[i] == 0)
                {
                    i = unspecifiedIndices[2];
                }
                else if (values[j] == 0)
                {
                    j = unspecifiedIndices[2];
                }

                int temp = values[i];
                values[i] = values[j];
                values[j] = temp;
            }

            return new BoardState(template.Width, template.Height, values);
        }

        public BoardState Generate(BoardTemplate template, BoardGoal goal)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            if (goal == null)
            {
                throw new ArgumentNullException(nameof(goal));
            }

            for (int i = 0; i < RetryCount; i++)
            {
                BoardState state = this.Generate(template);
                if (!state.Satisfies(goal))
                {
                    return state;
                }
            }

            throw new BoardGenerationException("Failed to generate a board that doesn't satisfy the specified goal.");
        }

        [NotNull]
        private static List<int> GetUnspecifiedIndices(int[] values)
        {
            List<int> positions = new List<int>();
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == 0)
                {
                    positions.Add(i);
                }
            }

            return positions;
        }

        [NotNull]
        private static List<int> GetUnspecifiedValues(int[] values)
        {
            List<int> missingValues = Enumerable.Range(0, values.Length).ToList();
            foreach (int v in values)
            {
                if (v != 0)
                {
                    missingValues.Remove(v);
                }
            }

            return missingValues;
        }
    }
}
