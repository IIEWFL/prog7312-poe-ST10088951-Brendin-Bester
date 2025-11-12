using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// A simple singly linked list implementation with Clear functionality.
    /// </summary>
    public class CustomLinkedList<T>
    {
        private LinkedListNode<T> head;
        private int count;

        /// <summary>
        /// Gets the number of elements in the list.
        /// </summary>
        public int Count => count;

        /// <summary>
        /// Adds a new item to the end of the list.
        /// </summary>
        public void Add(T item)
        {
            if (head == null)
            {
                head = new LinkedListNode<T>(item);
            }
            else
            {
                var current = head;
                while (current.Next != null)
                    current = current.Next;

                current.Next = new LinkedListNode<T>(item);
            }
            count++;
        }

        /// <summary>
        /// Enumerates through all items in the list.
        /// </summary>
        public IEnumerable<T> Enumerate()
        {
            var current = head;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        /// <summary>
        /// Removes all items from the list.
        /// </summary>
        public void Clear()
        {
            head = null;
            count = 0;
        }
    }
}