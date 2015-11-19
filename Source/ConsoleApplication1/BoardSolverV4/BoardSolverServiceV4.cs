using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;

namespace ConsoleApplication1.BoardSolverV4
{
    public sealed class BoardSolverServiceV4 : IBoardSolver
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

            PriorityQueueV4 openSet = new PriorityQueueV4();
            openSet.Enqueue(NodeV4.CreateInitialNode(state, goal));
            while (true)
            {
                NodeV4 current = openSet.Dequeue();
                if (current.State.Satisfies(goal))
                {
                    return GetPathFrom(current).Reverse().ToArray();
                }

                foreach (NodeV4 neighbor in current.GetNeighbors(goal))
                {
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Enqueue(neighbor);
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private static IEnumerable<Step> GetPathFrom(NodeV4 goalNode1)
        {
            for (NodeV4 current = goalNode1; current.ParentNode != null && current.PreviousStep != null; current = current.ParentNode)
            {
                yield return current.PreviousStep.Value;
            }
        }
    }
}
