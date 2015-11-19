using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;

namespace ConsoleApplication1.BoardSolverV5
{
    public sealed class BoardSolverServiceV5 : IBoardSolver
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

            PriorityQueueV5 openSet = new PriorityQueueV5();
            openSet.Enqueue(NodeV5.CreateInitialNode(state, goal));
            while (true)
            {
                NodeV5 current = openSet.Dequeue();
                if (current.EstimatedDistanceToGoal == 0)
                {
                    return GetPathFrom(current).Reverse().ToArray();
                }

                foreach (NodeV5 neighbor in current.GetNeighbors(goal))
                {
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Enqueue(neighbor);
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private static IEnumerable<Step> GetPathFrom(NodeV5 goalNode1)
        {
            for (NodeV5 current = goalNode1; current.ParentNode != null && current.PreviousStep != null; current = current.ParentNode)
            {
                yield return current.PreviousStep.Value;
            }
        }
    }
}
