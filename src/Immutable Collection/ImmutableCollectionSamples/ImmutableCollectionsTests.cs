#region Using

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;

#endregion // Using

namespace Sela.Samples
{
    [TestClass]
    public class ImmutableCollectionsTests
    {
        #region String_AsCharCollection_Test

        [TestMethod]
        public void String_AsCharCollection_Test()
        {
            var s1 = "Hello";
            var s2 = s1.Replace("e", "*");
            var s3 = s1.Remove(3, 1);
            var s4 = s1.Insert(3, "@@");

            Assert.IsFalse(s1 == s2);
            Assert.IsFalse(s3 == s2);
            Assert.IsFalse(s3 == s4);
        }

        #endregion // String_AsCharCollection_Test

        #region AddItem_Test

        [TestMethod]
        public void AddItem_Test()
        {
            // immutable collection starts with Empty or Create (no public Ctor)
            var col = ImmutableList<int>.Empty;
            var col1 = col.Add(1);
            var col2 = col1.Add(2);

            Assert.AreEqual(0, col.Count);
            Assert.AreEqual(1, col1.Count);
            Assert.AreEqual(2, col2.Count);
        }

        #endregion // AddItem_Test

        #region Create_Test

        [TestMethod]
        public void Create_Test()
        {
            var col = ImmutableList.Create<int>(1,2,3,4);

            Assert.AreEqual(4, col.Count);
        }

        #endregion // Create_Test

        #region ToImmutable_Test

        [TestMethod]
        public void ToImmutable_Test()
        {
            var source = Enumerable.Range(0, 10).ToList();
            var col = source.ToImmutableList();
            source.RemoveRange(0, 5);

            Assert.AreEqual(10, col.Count);
            Assert.AreEqual(5, source.Count);
        }

        #endregion // ToImmutable_Test

        #region ImmutableVsReadOnly_Test

        [TestMethod]
        public void ImmutableVsReadOnly_Test()
        {
            var source = Enumerable.Range(0, 10).ToList();
            var readonlyCollection = source.AsReadOnly();
            var immutableCollection = source.ToImmutableList();

            source.RemoveRange(0, 5); // will it affect the readonlyCollection

            Assert.AreEqual(10, immutableCollection.Count);
            Assert.AreEqual(5, readonlyCollection.Count);
            Assert.AreEqual(5, source.Count);
        }

        #endregion // ImmutableVsReadOnly_Test

        #region ImmutableVsArray_Test

        [TestMethod]
        public void ImmutableVsArray_Test()
        {
            var array = new int[] { 1, 2, 3, 4 };
            var immutable = array.ToImmutableList();

            array[1] = 100; // will it affect the Immutable Collection
            //immutable[1] = 999;
            Assert.AreEqual(2, immutable[1]);
            Assert.AreEqual(100, array[1]);
        }

        #endregion // ImmutableVsArray_Test

        #region CreateRange_Test

        [TestMethod]
        public void CreateRange_Test()
        {
            var col = ImmutableList.CreateRange<int>(Enumerable.Range(0, 1000));

            Assert.AreEqual(1000, col.Count);
        }

        #endregion // CreateRange_Test

        #region AddRange_Test

        [TestMethod]
        public void AddRange_Test()
        {
            var col = ImmutableList<int>.Empty;
            var col1 = col.AddRange(Enumerable.Range(0, 1000));

            Assert.AreEqual(0, col.Count);
            Assert.AreEqual(1000, col1.Count);
        }

        #endregion // AddRange_Test

        #region Builder_Test

        [TestMethod]
        public void Builder_Test()
        {
            var builder = ImmutableList.CreateBuilder<int>();
            for (int i = 0; i < 100; i++)
            {
                builder.Add(i);
            }
            var col = builder.ToImmutable();

            Assert.AreEqual(100, col.Count);
        }

        #endregion // Builder_Test

        #region ToBuilder_Test

        [TestMethod]
        public void ToBuilder_Test()
        {
            var col = ImmutableList<int>.Empty;
            var builder = col.ToBuilder();
            for (int i = 0; i < 100; i++)
            {
                builder.Add(i);
            }
            var col1 = builder.ToImmutable();

            Assert.AreEqual(0, col.Count);
            Assert.AreEqual(100, col1.Count);
        }

        #endregion // ToBuilder_Test

        #region RemoveRange_Test

        [TestMethod]
        public void RemoveRange_Test()
        {
            var col = ImmutableList.CreateRange<int>(Enumerable.Range(0, 1000));
            var col1 = col.RemoveRange(900, 100);

            Assert.AreEqual(1000, col.Count);
            Assert.AreEqual(900, col1.Count);
        }

        #endregion // RemoveRange_Test

        #region Builder_Remove_Test

        [TestMethod]
        public void Builder_Remove_Test()
        {
            var col = ImmutableList.CreateRange<int>(Enumerable.Range(0, 1000));
            var builder = col.ToBuilder();
            for (int i = 0; i < 100; i++)
            {
                builder.Remove(i * 10);
            }
            var col1 = builder.ToImmutable();
            Assert.AreEqual(1000, col.Count);
            Assert.AreEqual(900, col1.Count);
        }

        #endregion // Builder_Remove_Test
   }
}
