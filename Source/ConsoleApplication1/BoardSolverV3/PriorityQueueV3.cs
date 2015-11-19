using System;
using System.Collections.Generic;

namespace ConsoleApplication1.BoardSolverV3
{
    internal class PriorityQueueV3
    {
        // https://en.wikipedia.org/wiki/Binary_heap
        private readonly List<NodeV3> binaryHeap = new List<NodeV3>();

        private readonly HashSet<NodeV3> hashSet = new HashSet<NodeV3>();

        public bool Contains(NodeV3 node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return this.hashSet.Contains(node);
        }

        public void Enqueue(NodeV3 node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            int newIndex = this.binaryHeap.Count;
            this.binaryHeap.Add(node);
            this.hashSet.Add(node);

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

        public NodeV3 Dequeue()
        {
            if (this.binaryHeap.Count == 0)
            {
                throw new InvalidOperationException("The queue is empty.");
            }

            NodeV3 minNode = this.binaryHeap[0];
            NodeV3 lastNode = this.binaryHeap[this.binaryHeap.Count - 1];

            this.binaryHeap.RemoveAt(this.binaryHeap.Count - 1);
            this.hashSet.Remove(minNode);

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
