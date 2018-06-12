#region Using

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections;

#endregion // Using

namespace Sela.Samples
{
    #region RootWrapper (Factories)

    /// <summary>
    /// Factories methods for the RootWrapper
    /// </summary>
    public class RootWrapper
    {
        #region CreateRange

        /// <summary>
        /// Creates the range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static RootWrapper<T> CreateRange<T>(params T[] values)
        {
            string action = string.Format("CreateRange({0})", string.Join(",", values));
            var cur = ImmutableList.CreateRange(values);
            var instance = new RootWrapper<T>(cur, action);
            instance.CreateRoot(null);

            return instance;
        }

        #endregion // CreateRange

        #region ToBuilder

        /// <summary>
        /// Create builder.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        public static RootWrapper<T> ToBuilder<T>(RootWrapper<T> parent)
        {
            var cur = ((ImmutableList<T>)parent.Collection).ToBuilder();
            var instance = new RootWrapper<T>(cur, "ToBuilder");
            instance.CreateRoot(parent);

            return instance;
        }

        #endregion // ToBuilder

        #region Add

        /// <summary>
        /// Adds item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">The parent.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static RootWrapper<T> Add<T>(RootWrapper<T> parent, T value)
        {
            string action = string.Format("Add({0})", value);
            var cur = ((IImmutableList<T>)parent.Collection).Add(value);
            var instance = new RootWrapper<T>(cur, action);
            instance.CreateRoot(parent);

            return instance;
        }

        #endregion // Add

        #region AddRange

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">The parent.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static RootWrapper<T> AddRange<T>(RootWrapper<T> parent, params T[] values)
        {
            string action = string.Format("AddRange({0})", string.Join(",", values));
            var cur = ((IImmutableList<T>)parent.Collection).AddRange(values);
            var instance = new RootWrapper<T>(cur, action);
            instance.CreateRoot(parent);

            return instance;
        }

        #endregion // AddRange

        #region Insert

        /// <summary>
        /// Inserts.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">The parent.</param>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static RootWrapper<T> Insert<T>(RootWrapper<T> parent, int index, T value)
        {
            string action = string.Format("Insert({0}, {1})", index, value);
            var cur = ((IImmutableList<T>)parent.Collection).Insert(index, value);
            var instance = new RootWrapper<T>(cur, action);
            instance.CreateRoot(parent);

            return instance;
        }

        #endregion // Insert

        #region Remove

        /// <summary>
        /// Remove item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">The parent.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static RootWrapper<T> Remove<T>(RootWrapper<T> parent, T value)
        {
            string action = string.Format("Remove({0})", value);
            var cur = ((IImmutableList<T>)parent.Collection).Remove(value);
            var instance = new RootWrapper<T>(cur, action);
            instance.CreateRoot(parent);

            return instance;
        }

        #endregion // Remove

        #region RemoveAt

        /// <summary>
        /// Remove by index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">The parent.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static RootWrapper<T> RemoveAt<T>(RootWrapper<T> parent, int index)
        {
            string action = string.Format("RemoveAt({0})", index);
            var cur = ((IImmutableList<T>)parent.Collection).RemoveAt(index);
            var instance = new RootWrapper<T>(cur, action);
            instance.CreateRoot(parent);

            return instance;
        }

        #endregion // RemoveAt

        #region RemoveRange

        /// <summary>
        /// Removes the range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">The parent.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static RootWrapper<T> RemoveRange<T>(RootWrapper<T> parent, int index, int count)
        {
            string action = string.Format("RemoveRange({0}, {1})", index, count);
            var cur = ((IImmutableList<T>)parent.Collection).RemoveRange(index, count);
            var instance = new RootWrapper<T>(cur, action);
            instance.CreateRoot(parent);

            return instance;
        }

        #endregion // RemoveRange
    }

    #endregion // RootWrapper (Factories)

    public class RootWrapper<T> : IParentCompare, INotifyPropertyChanged, IEnumerable
    {
        private readonly BindingFlags FLAGS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
        private readonly HashSet<object> _map = new HashSet<object>();
        private const int ITEM_WIDTH = 35;
        private const int ITEM_HEIGHT = 35;

        #region Ctor

        internal RootWrapper(IEnumerable<T> collection, string action)
        {
            Collection = collection;
            Action = action;
        }

        #endregion // Ctor

        #region Root

        /// <summary>
        /// Gets the root.
        /// </summary>
        /// <value>
        /// The root.
        /// </value>
        public IBinaryTree<T> Root { get; private set; }

        #endregion // Root

        #region Action

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public string Action { get; private set; }

        #endregion // Action

        #region Collection

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <value>
        /// The collection.
        /// </value>
        public IEnumerable<T> Collection { get; private set; }

        #endregion // Collection

        #region IsExists

        /// <summary>
        /// Determines whether the specified item is exists.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool IsExists(object item)
        {
            return _map.Contains(item);
        }

        #endregion // IsExists

        #region Height

        private int _height;

        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        #endregion // Height

        #region Width

        private int _width;

        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged();
            }
        }

        #endregion // Width

        #region CreateRoot

        /// <summary>
        /// Creates the root.
        /// </summary>
        /// <param name="compareWithParent">The compare with parent.</param>
        internal void CreateRoot(IParentCompare compareWithParent)
        {
            object target = Collection.GetType().GetField("root", FLAGS).GetValue(Collection);

            var root = new BinaryTree<T>();
            _map.Add(target);
            Root = root;
            CrawlImmutableNode(target, root, compareWithParent);

            int x = 0;
            foreach (var node in root)
            {
                x += ITEM_WIDTH;
                ((BinaryTree<T>)node).X = x;
            }

            Width = x + ITEM_WIDTH;
            Height = root.Height * ITEM_HEIGHT;
        }

        #endregion // CreateRoot

        #region CrawlImmutableNode

        /// <summary>
        /// Crawls through the immutable node.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="node">The node.</param>
        /// <param name="compareWithParent"></param>
        public void CrawlImmutableNode(object target, BinaryTree<T> node, IParentCompare compareWithParent)
        {
            node.Value = (T)target.GetType().GetField("key", FLAGS).GetValue(target);
            node.IsFrozen = (bool)target.GetType().GetField("frozen", FLAGS).GetValue(target);
            node.Height = (int)target.GetType().GetField("height", FLAGS).GetValue(target);
            node.Count = (int)target.GetType().GetField("count", FLAGS).GetValue(target);
            node.HasChanged = !compareWithParent.IsExists(target);

            dynamic left = target.GetType().GetField("left", FLAGS).GetValue(target);
            bool isLeftEmpty = (bool)left.GetType().GetProperty("IsEmpty", FLAGS).GetValue(left);

            object right = target.GetType().GetField("right", FLAGS).GetValue(target);
            bool isRightEmpty = (bool)right.GetType().GetProperty("IsEmpty", FLAGS).GetValue(right);

            if (!isLeftEmpty)
            {
                _map.Add(left);
                var tmp = new BinaryTree<T>();
                tmp.Y = node.Y + ITEM_HEIGHT;
                node.Left = tmp;
                CrawlImmutableNode(left, tmp, compareWithParent);
            }

            if (!isRightEmpty)
            {
                _map.Add(right);
                var tmp = new BinaryTree<T>();
                tmp.Y = node.Y + ITEM_HEIGHT;
                node.Right = tmp;
                CrawlImmutableNode(right, tmp, compareWithParent);
            }
        }

        #endregion // CrawlImmutableNode

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion // PropertyChanged

        #region IEnumerable members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (Root == null)
                yield break;

            int offset = 10;
            foreach (var node in Root)
            {
                if (node.Left != null)
                    yield return new Connection { X = node.X + offset, XRelative = node.Left.X - node.X, Y = node.Y + offset, YRelative = node.Left.Y - node.Y };
                if (node.Right != null)
                    yield return new Connection { X = node.X + offset, XRelative = node.Right.X - node.X, Y = node.Y + offset, YRelative = node.Right.Y - node.Y };
            }
            foreach (var node in Root)
            {
                yield return node;                
            }
        }

        #endregion // IEnumerable members
    }
}
