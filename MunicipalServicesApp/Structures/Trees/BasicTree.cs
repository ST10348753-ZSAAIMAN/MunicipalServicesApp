using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.Structures.Trees
{
    /// <summary>
    /// Simple N-ary tree to demonstrate hierarchical categories.
    /// </summary>
    public class BasicTreeNode<T>
    {
        public T Value { get; set; }
        public List<BasicTreeNode<T>> Children { get; } = new List<BasicTreeNode<T>>();
        public BasicTreeNode(T value) => Value = value;
        public BasicTreeNode<T> AddChild(T value) { var c = new BasicTreeNode<T>(value); Children.Add(c); return c; }
    }

    public static class BasicTree
    {
        public static BasicTreeNode<string> BuildCategoryTree()
        {
            var root = new BasicTreeNode<string>("Root");
            var water = root.AddChild("Water");
            water.AddChild("Leak"); water.AddChild("Burst"); water.AddChild("Low Pressure");

            var electricity = root.AddChild("Electricity");
            electricity.AddChild("Outage"); electricity.AddChild("Fault"); electricity.AddChild("Meter");

            var roads = root.AddChild("Roads");
            roads.AddChild("Pothole"); roads.AddChild("Resurfacing"); roads.AddChild("Signage");

            var safety = root.AddChild("Community Safety");
            safety.AddChild("Streetlight"); safety.AddChild("Vandalism");

            return root;
        }
    }
}
