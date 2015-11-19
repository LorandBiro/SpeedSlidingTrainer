using System;
using System.Collections.Generic;

namespace ConsoleApplication1.BoardSolverV1
{
    internal class PriorityQueueV1
    {
        // https://en.wikipedia.org/wiki/Binary_heap
        private readonly List<NodeV1> binaryHeap = new List<NodeV1>();

        public void Enqueue(NodeV1 node)
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

        public NodeV1 Dequeue()
        {
            if (this.binaryHeap.Count == 0)
            {
                throw new InvalidOperationException("The queue is empty.");
            }

            NodeV1 minNode = this.binaryHeap[0];
            NodeV1 lastNode = this.binaryHeap[this.binaryHeap.Count - 1];

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
