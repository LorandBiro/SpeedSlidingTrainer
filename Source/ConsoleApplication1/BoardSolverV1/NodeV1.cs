using System;
using System.Collections.Generic;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;

namespace ConsoleApplication1.BoardSolverV1
{
    internal class NodeV1
    {
        private readonly int distanceFromInitialNode;

        private NodeV1(BoardState state, int estimatedDistanceToGoal)
        {
            this.State = state;
            this.Cost = estimatedDistanceToGoal;
        }

        private NodeV1(NodeV1 parentNode, Step previousStep, BoardState state, int distanceFromInitialNode, int estimatedDistanceToGoal)
        {
            this.ParentNode = parentNode;
            this.PreviousStep = previousStep;
            this.State = state;
            this.distanceFromInitialNode = distanceFromInitialNode;
            this.Cost = distanceFromInitialNode + estimatedDistanceToGoal;
        }

        public NodeV1 ParentNode { get; }

        public Step? PreviousStep { get; }

        public BoardState State { get; }

        public int Cost { get; }

        public static NodeV1 CreateInitialNode(BoardState state, BoardGoal goal)
        {
            return new NodeV1(state, GetManhattanDistance(state, goal));
        }

        public IEnumerable<NodeV1> GetNeighbors(BoardGoal goal)
        {
            if (this.State.CanSlideLeft)
            {
                yield return this.CreateNeighbor(Step.Left, this.State.SlideLeft(), goal);
            }

            if (this.State.CanSlideUp)
            {
                yield return this.CreateNeighbor(Step.Up, this.State.SlideUp(), goal);
            }

            if (this.State.CanSlideRight)
            {
                yield return this.CreateNeighbor(Step.Right, this.State.SlideRight(), goal);
            }

            if (this.State.CanSlideDown)
            {
                yield return this.CreateNeighbor(Step.Down, this.State.SlideDown(), goal);
            }
        }

        private static int GetManhattanDistance(BoardState state, BoardGoal goal)
        {
            int width = state.Width;
            int height = state.Height;

            int sum = 0;
            for (int i = 0; i < goal.TileCount; i++)
            {
                if (goal[i] == 0)
                {
                    continue;
                }

                for (int j = 0; j < state.TileCount; j++)
                {
                    if (state[j] == goal[i])
                    {
                        int x1 = i % width;
                        int y1 = i / height;
                        int x2 = j % width;
                        int y2 = j / height;

                        sum += Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
                        break;
                    }
                }
            }

            return sum;
        }

        private NodeV1 CreateNeighbor(Step previousStep, BoardState newState, BoardGoal goal)
        {
            return new NodeV1(this, previousStep, newState, this.distanceFromInitialNode + 1, GetManhattanDistance(newState, goal));
        }
    }
}
