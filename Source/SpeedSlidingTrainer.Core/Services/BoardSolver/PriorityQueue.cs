using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SpeedSlidingTrainer.Core.Services.BoardSolver
{
    internal class PriorityQueue
    {
        // https://en.wikipedia.org/wiki/Binary_heap
        private readonly List<Node> binaryHeap = new List<Node>();

        public int Count
        {
            get { return this.binaryHeap.Count; }
        }

        public void Enqueue([NotNull] Node node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            int newIndex = this.binaryHeap.Count;
            this.binaryHeap.Add(node);

            while (newIndex > 0)
            {
                int parentIndex = (newIndex - 1) / 2;
                if (this.binaryHeap[parentIndex].Cost <= node.Cost)
                {
                    break;
                }

                this.binaryHeap[newIndex] = this.binaryHeap[parentIndex];
                newIndex = parentIndex;
            }

            this.binaryHeap[newIndex] = node;
        }

        [NotNull]
        public Node Dequeue()
        {
            if (this.binaryHeap.Count == 0)
            {
                throw new InvalidOperationException("The queue is empty.");
            }

            Node minNode = this.binaryHeap[0];
            Node lastNode = this.binaryHeap[this.binaryHeap.Count - 1];

            this.binaryHeap.RemoveAt(this.binaryHeap.Count - 1);

            if (this.binaryHeap.Count == 0)
            {
                return minNode;
            }

            int targetIndex = 0;
            int firstIndexWithoutChild = this.binaryHeap.Count / 2;
            while (targetIndex < firstIndexWithoutChild)
            {
                int childIndex = (2 * targetIndex) + 1;
                if (childIndex < (this.binaryHeap.Count - 1) && this.binaryHeap[childIndex + 1].Cost < this.binaryHeap[childIndex].Cost)
                {
                    childIndex++;
                }

                if (lastNode.Cost <= this.binaryHeap[childIndex].Cost)
                {
                    break;
                }

                this.binaryHeap[targetIndex] = this.binaryHeap[childIndex];
                targetIndex = childIndex;
            }

            this.binaryHeap[targetIndex] = lastNode;
            return minNode;
        }
    }
}
