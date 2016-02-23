using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;

namespace SpeedSlidingTrainer.Core.Services.BoardSolver
{
    public sealed class BoardSolverService : IBoardSolverService
    {
        public Step[][] GetSolution(BoardState state, BoardGoal goal, CancellationToken cancellationToken)
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

            List<Step[]> solutions = new List<Step[]>();
            int targetCost = int.MaxValue;

            PriorityQueue openSet = new PriorityQueue();
            openSet.Enqueue(Node.CreateInitialNode(state, goal));
            HashSet<Node> closedSet = new HashSet<Node>();
            while (true)
            {
                Node current = openSet.Dequeue();
                closedSet.Add(current);
                if (current.Cost > targetCost)
                {
                    break;
                }

                if (current.EstimatedDistanceToGoal == 0)
                {
                    Step[] solution = GetPathFrom(current).Reverse().ToArray();
                    solutions.Add(solution);
                    if (solution.Length < targetCost)
                    {
                        targetCost = solution.Length;
                    }

                    if (openSet.Count == 0)
                    {
                        break;
                    }

                    continue;
                }

                foreach (Node neighbor in current.GetNeighbors(goal))
                {
                    if (!closedSet.Contains(neighbor))
                    {
                        openSet.Enqueue(neighbor);
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();
            }

            return solutions.ToArray();
        }

        private static IEnumerable<Step> GetPathFrom(Node goalNode)
        {
            for (Node current = goalNode; current.ParentNode != null && current.PreviousStep != null; current = current.ParentNode)
            {
                yield return current.PreviousStep.Value;
            }
        }
    }
}
