using System;
using System.Collections;
using System.Collections.Generic;

//Stack Overflow. (n.d.).Newest ‘min - heap’ Questions. [online] Available at: https://stackoverflow.com/questions/tagged/min-heap.

namespace MunicipalServicesApp.DataStructures
{
    public class MinHeap<T>
    {
        private readonly List<T> heap = new List<T>();
        private readonly Comparison<T> comparison;

        public MinHeap(Comparison<T> comparison)
        {
            this.comparison = comparison ?? throw new ArgumentNullException(nameof(comparison));
        }

        public int Count => heap.Count;

        public void Add(T item)
        {
            heap.Add(item);
            HeapifyUp(heap.Count - 1);
        }

        public T Peek()
        {
            return heap.Count == 0 ? default(T) : heap[0];
        }

        public T Pop()
        {
            if (heap.Count == 0) return default(T);
            var root = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);
            HeapifyDown(0);
            return root;
        }

        public IEnumerable<T> PeekN(int n)
        {
            var temp = new MinHeap<T>(comparison);
            foreach (var i in heap) temp.Add(i);
            var results = new List<T>();
            for (int i = 0; i < n && temp.Count > 0; i++) results.Add(temp.Pop());
            return results;
        }

        public void Remove(T item)
        {
            int idx = heap.IndexOf(item);
            if (idx < 0) return;
            heap[idx] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);
            if (idx < heap.Count)
            {
                HeapifyUp(idx);
                HeapifyDown(idx);
            }
        }

        private void HeapifyUp(int i)
        {
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (comparison(heap[i], heap[parent]) >= 0) break;
                Swap(i, parent);
                i = parent;
            }
        }

        private void HeapifyDown(int i)
        {
            int left = 2 * i + 1;
            while (left < heap.Count)
            {
                int smallest = left;
                int right = left + 1;
                if (right < heap.Count && comparison(heap[right], heap[left]) < 0) smallest = right;
                if (comparison(heap[smallest], heap[i]) >= 0) break;
                Swap(i, smallest);
                i = smallest;
                left = 2 * i + 1;
            }
        }

        private void Swap(int a, int b)
        {
            var t = heap[a];
            heap[a] = heap[b];
            heap[b] = t;
        }
    }
}