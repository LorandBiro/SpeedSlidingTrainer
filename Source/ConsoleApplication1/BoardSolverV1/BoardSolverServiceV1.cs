using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;

namespace ConsoleApplication1.BoardSolverV1
{
    public sealed class BoardSolverServiceV1 : IBoardSolver
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

            PriorityQueueV1 openSet = new PriorityQueueV1();
            openSet.Enqueue(NodeV1.CreateInitialNode(state, goal));
            while (true)
            {
                NodeV1 current = openSet.Dequeue();
                if (current.State.Satisfies(goal))
                {
                    return new List<Step[]> { GetPathFrom(current).Reverse().ToArray() };
                }

                foreach (NodeV1 neighbor in current.GetNeighbors(goal))
                {
                    openSet.Enqueue(neighbor);
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private static IEnumerable<Step> GetPathFrom(NodeV1 goalNode1)
        {
            for (NodeV1 current = goalNode1; current.ParentNode != null && current.PreviousStep != null; current = current.ParentNode)
            {
                yield return current.PreviousStep.Value;
            }
        }
    }
}
