using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.DataStructures
{
    // AVL tree keyed by long (e.g. CreatedAt.Ticks)
    //GeeksforGeeks (2023). AVL Tree Data Structure. [online] GeeksforGeeks. Available at: https://www.geeksforgeeks.org/dsa/introduction-to-avl-tree/.
    public class AvlTree<T>
    {
        private class Node
        {
            public long Key;
            public T Value;
            public Node Left, Right;
            public int Height;
            public Node(long key, T value) { Key = key; Value = value; Height = 1; }
        }

        private Node root;
        private readonly Func<T, long> keySelector;

        public AvlTree(Func<T, long> keySelector)
        {
            this.keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        }

        public void Insert(T value) => root = InsertRec(root, keySelector(value), value);

        private Node InsertRec(Node node, long key, T value)
        {
            if (node == null) return new Node(key, value);
            if (key < node.Key) node.Left = InsertRec(node.Left, key, value);
            else if (key > node.Key) node.Right = InsertRec(node.Right, key, value);
            else node.Value = value;

            UpdateHeight(node);
            return Balance(node);
        }

        public void Delete(T value) => root = DeleteRec(root, keySelector(value));

        private Node DeleteRec(Node node, long key)
        {
            if (node == null) return null;
            if (key < node.Key) node.Left = DeleteRec(node.Left, key);
            else if (key > node.Key) node.Right = DeleteRec(node.Right, key);
            else
            {
                if (node.Left == null) return node.Right;
                if (node.Right == null) return node.Left;
                var min = FindMin(node.Right);
                node.Key = min.Key; node.Value = min.Value;
                node.Right = DeleteRec(node.Right, min.Key);
            }
            UpdateHeight(node);
            return Balance(node);
        }

        private Node FindMin(Node node)
        {
            while (node.Left != null) node = node.Left;
            return node;
        }

        private int Height(Node n) => n?.Height ?? 0;
        private void UpdateHeight(Node n) => n.Height = Math.Max(Height(n.Left), Height(n.Right)) + 1;
        private int BalanceFactor(Node n) => Height(n.Left) - Height(n.Right);

        private Node RotateRight(Node y)
        {
            var x = y.Left;
            var T2 = x.Right;
            x.Right = y;
            y.Left = T2;
            UpdateHeight(y);
            UpdateHeight(x);
            return x;
        }

        private Node RotateLeft(Node x)
        {
            var y = x.Right;
            var T2 = y.Left;
            y.Left = x;
            x.Right = T2;
            UpdateHeight(x);
            UpdateHeight(y);
            return y;
        }

        private Node Balance(Node node)
        {
            var bf = BalanceFactor(node);
            if (bf > 1)
            {
                if (BalanceFactor(node.Left) < 0) node.Left = RotateLeft(node.Left);
                return RotateRight(node);
            }
            if (bf < -1)
            {
                if (BalanceFactor(node.Right) > 0) node.Right = RotateRight(node.Right);
                return RotateLeft(node);
            }
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