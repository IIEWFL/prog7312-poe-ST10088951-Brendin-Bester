using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.DataStructures
{
    // Generic BST keyed by string (keySelector produces a comparable string key)
    //GeeksforGeeks (2024). Binary Search Tree. [online] GeeksforGeeks. Available at: https://www.geeksforgeeks.org/dsa/binary-search-tree-data-structure/.‌
    public class BinarySearchTree<T>
    {
        private class Node
        {
            public string Key;
            public T Value;
            public Node Left, Right;
            public Node(string key, T value) { Key = key; Value = value; }
        }

        private Node root;
        private readonly Func<T, string> keySelector;

        public BinarySearchTree(Func<T, string> keySelector)
        {
            this.keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        }

        public void Insert(T value)
        {
            var key = keySelector(value);
            root = InsertRec(root, key, value);
        }

        private Node InsertRec(Node node, string key, T value)
        {
            if (node == null) return new Node(key, value);
            var cmp = string.Compare(key, node.Key, StringComparison.OrdinalIgnoreCase);
            if (cmp < 0) node.Left = InsertRec(node.Left, key, value);
            else if (cmp > 0) node.Right = InsertRec(node.Right, key, value);
            else node.Value = value; // replace
            return node;
        }

        public T Find(string key)
        {
            var node = root;
            while (node != null)
            {
                var cmp = string.Compare(key, node.Key, StringComparison.OrdinalIgnoreCase);
                if (cmp == 0) return node.Value;
                node = cmp < 0 ? node.Left : node.Right;
            }
            return default(T);
        }

        public void Delete(T value)
        {
            var key = keySelector(value);
            root = DeleteRec(root, key);
        }

        private Node DeleteRec(Node node, string key)
        {
            if (node == null) return null;
            var cmp = string.Compare(key, node.Key, StringComparison.OrdinalIgnoreCase);
            if (cmp < 0) node.Left = DeleteRec(node.Left, key);
            else if (cmp > 0) node.Right = DeleteRec(node.Right, key);
            else
            {
                if (node.Left == null) return node.Right;
                if (node.Right == null) return node.Left;
                var min = FindMin(node.Right);
                node.Key = min.Key;
                node.Value = min.Value;
                node.Right = DeleteRec(node.Right, min.Key);
            }
            return node;
        }

        private Node FindMin(Node node)
        {
            while (node.Left != null) node = node.Left;
            return node;
        }

        public IEnumerable<T> InOrder()
        {
            var list = new List<T>();
            InOrderRec(root, list);
            return list;
        }

        private void InOrderRec(Node node, List<T> list)
        {
            if (node == null) return;
            InOrderRec(node.Left, list);
            list.Add(node.Value);
            InOrderRec(node.Right, list);
        }
    }
}
