using System;
using System.Collections.Generic;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;

namespace ConsoleApplication1.BoardSolverV5
{
    internal class NodeV5 : IEquatable<NodeV5>
    {
        private readonly int distanceFromInitialNode;

        private NodeV5(BoardStateV5 state, int estimatedDistanceToGoal)
        {
            this.State = state;
            this.EstimatedDistanceToGoal = estimatedDistanceToGoal;
            this.Cost = estimatedDistanceToGoal;
        }

        private NodeV5(NodeV5 parentNode, Step previousStep, BoardStateV5 state, int distanceFromInitialNode, int estimatedDistanceToGoal)
        {
            this.ParentNode = parentNode;
            this.PreviousStep = previousStep;
            this.State = state;
            this.distanceFromInitialNode = distanceFromInitialNode;
            this.EstimatedDistanceToGoal = estimatedDistanceToGoal;
            this.Cost = distanceFromInitialNode + estimatedDistanceToGoal;
        }

        public NodeV5 ParentNode { get; }

        public Step? PreviousStep { get; }

        public BoardStateV5 State { get; }

        public int Cost { get; }

        public int EstimatedDistanceToGoal { get; }

        public static NodeV5 CreateInitialNode(BoardState state, BoardGoal goal)
        {
            int[] values = new int[state.TileCount];
            int eye = 0;
            for (int i = 0; i < state.TileCount; i++)
            {
                if (state[i] == 0)
                {
                    eye = i;
                    values[i] = -1;
                    continue;
                }

                for (int j = 0; j < goal.TileCount; j++)
                {
                    if (state[i] == goal[j])
                    {
                        values[i] = state[i];
                        break;
                    }
                }
            }

            BoardStateV5 myState = new BoardStateV5(state.Width, state.Height, values, eye);
            return new NodeV5(myState, GetManhattanDistance(myState, goal));
        }

        public IEnumerable<NodeV5> GetNeighbors(BoardGoal goal)
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

        public bool Equals(NodeV5 other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            for (int i = 0; i < this.State.Values.Length; i++)
            {
                if (this.State.Values[i] != other.State.Values[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as NodeV5);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 0;
                for (int i = 0; i < this.State.Values.Length; i++)
                {
                    hashCode = (hashCode * 397) ^ this.State.Values[i];
                }

                return hashCode;
            }
        }

        private static int GetManhattanDistance(BoardStateV5 state, BoardGoal goal)
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

                for (int j = 0; j < goal.TileCount; j++)
                {
                    if (state.Values[j] == goal[i])
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

        private NodeV5 CreateNeighbor(Step previousStep, BoardStateV5 newState, BoardGoal goal)
        {
            return new NodeV5(this, previousStep, newState, this.distanceFromInitialNode + 1, GetManhattanDistance(newState, goal));
        }
    }
}
