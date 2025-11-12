using System;

namespace MunicipalServicesApp.Structures.Trees
{
    /// <summary>
    /// Minimal binary tree for traversal demo (not ordered).
    /// </summary>
    public class BinaryTreeNode<T>
    {
        public T Value;
        public BinaryTreeNode<T> Left, Right;
        public BinaryTreeNode(T value) { Value = value; }
    }

    public static class BinaryTree
    {
        public static void PreOrder<T>(BinaryTreeNode<T> n, Action<T> visit)
        {
            if (n == null) return;
            visit(n.Value);
            PreOrder(n.Left, visit);
            PreOrder(n.Right, visit);
        }
        public static void InOrder<T>(BinaryTreeNode<T> n, Action<T> visit)
        {
            if (n == null) return;
            InOrder(n.Left, visit);
            visit(n.Value);
            InOrder(n.Right, visit);
        }
        public static void PostOrder<T>(BinaryTreeNode<T> n, Action<T> visit)
        {
            if (n == null) return;
            PostOrder(n.Left, visit);
            PostOrder(n.Right, visit);
            visit(n.Value);
        }
    }
}
