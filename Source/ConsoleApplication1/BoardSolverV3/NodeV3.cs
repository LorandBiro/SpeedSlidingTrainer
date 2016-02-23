using System;
using System.Collections.Generic;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;

namespace ConsoleApplication1.BoardSolverV3
{
    internal class NodeV3 : IEquatable<NodeV3>
    {
        private readonly int distanceFromInitialNode;

        private NodeV3(BoardState state, int estimatedDistanceToGoal)
        {
            this.State = state;
            this.Cost = estimatedDistanceToGoal;
        }

        private NodeV3(NodeV3 parentNode, Step previousStep, BoardState state, int distanceFromInitialNode, int estimatedDistanceToGoal)
        {
            this.ParentNode = parentNode;
            this.PreviousStep = previousStep;
            this.State = state;
            this.distanceFromInitialNode = distanceFromInitialNode;
            this.Cost = distanceFromInitialNode + estimatedDistanceToGoal;
        }

        public NodeV3 ParentNode { get; }

        public Step? PreviousStep { get; }

        public BoardState State { get; }

        public int Cost { get; }

        public static NodeV3 CreateInitialNode(BoardState state, BoardGoal goal)
        {
            return new NodeV3(state, GetManhattanDistance(state, goal));
        }

        public IEnumerable<NodeV3> GetNeighbors(BoardGoal goal)
        {
            if (this.State.CanSlideLeft && this.PreviousStep != Step.Right)
            {
                yield return this.CreateNeighbor(Step.Left, this.State.SlideLeft(), goal);
            }

            if (this.State.CanSlideUp && this.PreviousStep != Step.Down)
            {
                yield return this.CreateNeighbor(Step.Up, this.State.SlideUp(), goal);
            }

            if (this.State.CanSlideRight && this.PreviousStep != Step.Left)
            {
                yield return this.CreateNeighbor(Step.Right, this.State.SlideRight(), goal);
            }

            if (this.State.CanSlideDown && this.PreviousStep != Step.Up)
            {
                yield return this.CreateNeighbor(Step.Down, this.State.SlideDown(), goal);
            }
        }

        public bool Equals(NodeV3 other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            for (int i = 0; i < this.State.TileCount; i++)
            {
                if (this.State[i] != other.State[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as NodeV3);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 0;
                for (int i = 0; i < this.State.TileCount; i++)
                {
                    hashCode = (hashCode * 397) ^ this.State[i];
                }

                return hashCode;
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

        private NodeV3 CreateNeighbor(Step previousStep, BoardState newState, BoardGoal goal)
        {
            return new NodeV3(this, previousStep, newState, this.distanceFromInitialNode + 1, GetManhattanDistance(newState, goal));
        }
    }
}
