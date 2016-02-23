using System;
using System.Collections.Generic;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;

namespace ConsoleApplication1.BoardSolverV4
{
    internal class NodeV4 : IEquatable<NodeV4>
    {
        private readonly int distanceFromInitialNode;

        private NodeV4(BoardStateV4 state, int estimatedDistanceToGoal)
        {
            this.State = state;
            this.Cost = estimatedDistanceToGoal;
        }

        private NodeV4(NodeV4 parentNode, Step previousStep, BoardStateV4 state, int distanceFromInitialNode, int estimatedDistanceToGoal)
        {
            this.ParentNode = parentNode;
            this.PreviousStep = previousStep;
            this.State = state;
            this.distanceFromInitialNode = distanceFromInitialNode;
            this.Cost = distanceFromInitialNode + estimatedDistanceToGoal;
        }

        public NodeV4 ParentNode { get; }

        public Step? PreviousStep { get; }

        public BoardStateV4 State { get; }

        public int Cost { get; }

        public static NodeV4 CreateInitialNode(BoardState state, BoardGoal goal)
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

            BoardStateV4 myState = new BoardStateV4(state.Width, state.Height, values, eye);
            return new NodeV4(myState, GetManhattanDistance(myState, goal));
        }

        public IEnumerable<NodeV4> GetNeighbors(BoardGoal goal)
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

        public bool Equals(NodeV4 other)
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
            return this.Equals(obj as NodeV4);
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

        private static int GetManhattanDistance(BoardStateV4 state, BoardGoal goal)
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

        private NodeV4 CreateNeighbor(Step previousStep, BoardStateV4 newState, BoardGoal goal)
        {
            return new NodeV4(this, previousStep, newState, this.distanceFromInitialNode + 1, GetManhattanDistance(newState, goal));
        }
    }
}
