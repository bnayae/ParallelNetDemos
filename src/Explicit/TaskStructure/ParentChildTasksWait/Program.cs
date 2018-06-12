#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;

#endregion // Using

/// the sample demonstrate parent child relationship
/// using the Wait operation

namespace Tpl.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Node root = CreateTree(100);

            root.Do(node => node.Data *= 2);
            root.Do(node =>
            {
                if (node.Left == null)
                    Debugger.Break(); // check the parallel task window
            });

            Console.ReadKey();
        }

        #region CreateTree

        /// <summary>
        /// initialize the tree
        /// </summary>
        private static Node CreateTree(int size)
        {
            Node root = new Node(size / 2);
            var rnd = new Random(42);
            var numbers = Enumerable.Range(0, size).ToList();
            numbers.RemoveAt(root.Data);
            do
            {
                int index = rnd.Next(numbers.Count);
                int value = numbers[index];
                root.Add(value);
                numbers.RemoveAt(index);
            } while (numbers.Count != 0);

            return root;
        }

        #endregion // CreateTree}
    }

    public class Node
    {
        public Node (int data)
	    {
            Data = data;
	    }

        public int Data { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }

        public void Add (int data)
        {
            if (data < this.Data)
            {
                if (Left == null)
                    Left = new Node(data);
                else
                    Left.Add (data);
            }
            else
            {
                if (Right == null)
                    Right = new Node(data);
                else
                    Right.Add (data);
            }
        }

        public void Do(Action<Node> action)
        {
            //TaskCreationOptions options = TaskCreationOptions.None;
            TaskCreationOptions options = TaskCreationOptions.AttachedToParent;

            var t1 = Task.Factory.StartNew(state => action(this), Data + " Operate action", options);
            var t2 = Task.Factory.StartNew(stateLeft => 
                {
                    if (Left != null)
                        Left.Do(action);
                }, Data + " Left", options);
            var t3 = Task.Factory.StartNew(stateRight => {
                    if (Right != null)
                        Right.Do(action);
            }, Data + " Right", options);
            Task.WaitAll(t1, t2, t3);
        }
    }
}
