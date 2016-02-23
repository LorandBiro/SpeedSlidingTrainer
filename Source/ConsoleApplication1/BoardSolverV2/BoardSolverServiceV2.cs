using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;

namespace ConsoleApplication1.BoardSolverV2
{
    public sealed class BoardSolverServiceV2 : IBoardSolver
    {
        public List<Step[]> GetSolution(BoardState state, BoardGoal goal, CancellationToken cancellationToken)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (goal == null)
            {
                throw new ArgumentNullException(nameof(goal));
            }

            if (state.Width != goal.Width || state.Height != goal.Height)
            {
                throw new ArgumentException($"The state ({state.Width}x{state.Height}) and the goal ({goal.Width}x{goal.Height}) has different sizes.");
            }

            PriorityQueueV2 openSet = new PriorityQueueV2();
            openSet.Enqueue(NodeV2.CreateInitialNode(state, goal));
            while (true)
            {
                NodeV2 current = openSet.Dequeue();
                if (current.State.Satisfies(goal))
                {
                    return new List<Step[]> { GetPathFrom(current).Reverse().ToArray() };
                }

                foreach (NodeV2 neighbor in current.GetNeighbors(goal))
                {
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Enqueue(neighbor);
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private static IEnumerable<Step> GetPathFrom(NodeV2 goalNode1)
        {
            for (NodeV2 current = goalNode1; current.ParentNode != null && current.PreviousStep != null; current = current.ParentNode)
            {
                yield return current.PreviousStep.Value;
            }
        }
    }
}
