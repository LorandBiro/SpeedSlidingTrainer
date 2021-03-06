﻿using System;
using System.Collections.Generic;

namespace ConsoleApplication1.BoardSolverV2
{
    internal class PriorityQueueV2
    {
        // https://en.wikipedia.org/wiki/Binary_heap
        private readonly List<NodeV2> binaryHeap = new List<NodeV2>();

        private readonly HashSet<NodeV2> hashSet = new HashSet<NodeV2>();

        public bool Contains(NodeV2 node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return this.hashSet.Contains(node);
        }

        public void Enqueue(NodeV2 node)
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

        public NodeV2 Dequeue()
        {
            if (this.binaryHeap.Count == 0)
            {
                throw new InvalidOperationException("The queue is empty.");
            }

            NodeV2 minNode = this.binaryHeap[0];
            NodeV2 lastNode = this.binaryHeap[this.binaryHeap.Count - 1];

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
