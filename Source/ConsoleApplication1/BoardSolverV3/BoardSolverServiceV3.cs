using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;

namespace ConsoleApplication1.BoardSolverV3
{
    public sealed class BoardSolverServiceV3 : IBoardSolver
    {
        public Step[] GetSolution(BoardState state, BoardGoal goal, CancellationToken cancellationToken)
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

            PriorityQueueV3 openSet = new PriorityQueueV3();
            openSet.Enqueue(NodeV3.CreateInitialNode(state, goal));
            while (true)
            {
                NodeV3 current = openSet.Dequeue();
                if (current.State.Satisfies(goal))
                {
                    return GetPathFrom(current).Reverse().ToArray();
                }

                foreach (NodeV3 neighbor in current.GetNeighbors(goal))
                {
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Enqueue(neighbor);
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private static IEnumerable<Step> GetPathFrom(NodeV3 goalNode1)
        {
            for (NodeV3 current = goalNode1; current.ParentNode != null && current.PreviousStep != null; current = current.ParentNode)
            {
                yield return current.PreviousStep.Value;
            }
        }
    }
}
