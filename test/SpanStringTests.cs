// Copyright (c) Erik Mavrinac, https://github.com/erikma. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OptimizedStrings.Tests
{
    [TestClass]
    public class SpanStringTests
    {
        [TestMethod]
        public void SpanStringEmptyIsEmpty()
        {
            Assert.AreEqual(0, SpanString.Empty.Length);
            Assert.IsTrue(SpanString.Empty.IsWhiteSpace());
        }

        [TestMethod]
        public void SpanString1FromFullString()
        {
            var ss = new SpanString1("ABCDEFG");
            Assert.AreEqual(7, ss.Length);
            Assert.AreEqual(0, ss.CompareTo(new SpanString1("ABCDEFG")));

            Assert.AreEqual(ss.GetHashCode(), (new SpanString1("ABCDEFG")).GetHashCode());
            Assert.AreEqual(ss.GetHashCodeIgnoreCase(), (new SpanString1("ABCDEFG")).GetHashCodeIgnoreCase());
            Assert.AreEqual(ss.GetHashCodeIgnoreCase(), (new SpanString1("abcdefg")).GetHashCodeIgnoreCase());

            Assert.AreNotEqual(0, ss.CompareTo(new SpanString1("abcdefg")));
            Assert.AreNotEqual(ss.GetHashCode(), (new SpanString1("abcdefg")).GetHashCode());
            Assert.AreEqual(0, ss.CompareTo(new SpanString1("abcdefg"), true));
        }

        [TestMethod]
        public void SpanString1FromTail()
        {
            var ss = new SpanString1("ABCDEFG", 2);
            Assert.AreEqual(5, ss.Length);
            Assert.AreEqual(0, ss.CompareTo(new SpanString1("CDEFG")));

            Assert.AreEqual(ss.GetHashCode(), (new SpanString1("CDEFG")).GetHashCode());
            Assert.AreEqual(ss.GetHashCodeIgnoreCase(), (new SpanString1("CDEFG")).GetHashCodeIgnoreCase());
            Assert.AreEqual(ss.GetHashCodeIgnoreCase(), (new SpanString1("cdefg")).GetHashCodeIgnoreCase());

            Assert.AreNotEqual(0, ss.CompareTo(new SpanString1("cdefg")));
            Assert.AreNotEqual(ss.GetHashCode(), (new SpanString1("cdefg")).GetHashCode());
            Assert.AreEqual(0, ss.CompareTo(new SpanString1("cdefg"), true));
        }

        [TestMethod]
        public void SpanString1FromSlice()
        {
            var ss = new SpanString1("ABCDEFG", 2, 3);
            Assert.AreEqual(3, ss.Length);
            Assert.AreEqual(0, ss.CompareTo(new SpanString1("CDE")));

            Assert.AreEqual(ss.GetHashCode(), (new SpanString1("CDE")).GetHashCode());
            Assert.AreEqual(ss.GetHashCodeIgnoreCase(), (new SpanString1("CDE")).GetHashCodeIgnoreCase());
            Assert.AreEqual(ss.GetHashCodeIgnoreCase(), (new SpanString1("cde")).GetHashCodeIgnoreCase());

            Assert.AreNotEqual(0, ss.CompareTo(new SpanString1("cde")));
            Assert.AreNotEqual(ss.GetHashCode(), (new SpanString1("cde")).GetHashCode());
            Assert.AreEqual(0, ss.CompareTo(new SpanString1("cde"), true));
        }

        [TestMethod]
        public void SpanStringFactory()
        {
            ISpanString ss1 = SpanString.Create("abcdefg");
            Assert.AreEqual(7, ss1.Length);

            ISpanString ss2 = SpanString.Create("abc", "defg");
            Assert.AreEqual(7, ss2.Length);

            Assert.AreEqual(ss1.GetHashCode(), ss2.GetHashCode());
            Assert.AreEqual(ss1.GetHashCodeIgnoreCase(), ss2.GetHashCodeIgnoreCase());
        }

        [TestMethod]
        public void SpanString1AsDefaultEquatableDictionaryKey()
        {
            var d = new Dictionary<SpanString1, int>();
            var a = new SpanString1("a");
            var b = new SpanString1("bb");
            var aCap = new SpanString1("A");
            d[a] = 0;
            d[b] = 1;
            d[aCap] = 2;
            Assert.AreEqual(3, d.Count);
            Assert.AreEqual(2, d[aCap]);
            Assert.AreEqual(1, d[b]);
            Assert.AreEqual(0, d[a]);
        }

        [TestMethod]
        public void SpanString1AsOrdinalDictionaryKey()
        {
            var d = new Dictionary<SpanString1, int>(SpanStringComparer.Ordinal);
            var a = new SpanString1("a");
            var b = new SpanString1("bb");
            var aCap = new SpanString1("A");
            d[a] = 0;
            d[b] = 1;
            d[aCap] = 2;
            Assert.AreEqual(3, d.Count);
            Assert.AreEqual(2, d[aCap]);
            Assert.AreEqual(1, d[b]);
            Assert.AreEqual(0, d[a]);
        }

        [TestMethod]
        public void SpanString1AsOrdinalIgnoreCaseDictionaryKey()
        {
            var d = new Dictionary<SpanString1, int>(SpanStringComparer.OrdinalIgnoreCase);
            var a = new SpanString1("a");
            var b = new SpanString1("bb");
            var aCap = new SpanString1("A");
            d[a] = 0;
            d[b] = 1;
            d[aCap] = 2;
            Assert.AreEqual(2, d.Count);
            Assert.AreEqual(2, d[aCap]);
            Assert.AreEqual(1, d[b]);
            Assert.AreEqual(2, d[a]);
        }

        [TestMethod]
        public void SpanString2AsDefaultEquatableDictionaryKey()
        {
            var d = new Dictionary<SpanString2, int>();
            var a = new SpanString2("a", "b");
            var b = new SpanString2("b", "c");
            var aCap = new SpanString2("A", "B");
            d[a] = 0;
            d[b] = 1;
            d[aCap] = 2;
            Assert.AreEqual(3, d.Count);
            Assert.AreEqual(2, d[aCap]);
            Assert.AreEqual(1, d[b]);
            Assert.AreEqual(0, d[a]);
        }

        [TestMethod]
        public void SpanString2AsOrdinalDictionaryKey()
        {
            var d = new Dictionary<SpanString2, int>(SpanStringComparer.Ordinal);
            var a = new SpanString2("a", "b");
            var b = new SpanString2("b", "c");
            var aCap = new SpanString2("A", "B");
            d[a] = 0;
            d[b] = 1;
            d[aCap] = 2;
            Assert.AreEqual(3, d.Count);
            Assert.AreEqual(2, d[aCap]);
            Assert.AreEqual(1, d[b]);
            Assert.AreEqual(0, d[a]);
        }

        [TestMethod]
        public void SpanString2AsOrdinalIgnoreCaseDictionaryKey()
        {
            var d = new Dictionary<SpanString2, int>(SpanStringComparer.OrdinalIgnoreCase);
            var a = new SpanString2("a", "b");
            var b = new SpanString2("b", "c");
            var aCap = new SpanString2("A", "B");
            d[a] = 0;
            d[b] = 1;
            d[aCap] = 2;
            Assert.AreEqual(2, d.Count);
            Assert.AreEqual(2, d[aCap]);
            Assert.AreEqual(1, d[b]);
            Assert.AreEqual(2, d[a]);
        }
    }
}
