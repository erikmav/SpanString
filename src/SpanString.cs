// Copyright (c) Erik Mavrinac, https://github.com/erikma. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace OptimizedStrings
{
    /// <summary>
    /// A factory class for SpanStrings.
    /// </summary>
    public static class SpanString
    {
        /// <summary>
        /// Gets a SpanString containing only the empty string.
        /// </summary>
        /// <remarks>This is a boxed <see cref="SpanString1"/>.</remarks>
        public static ISpanString Empty { get; } = new SpanString1("");

        /// <summary>
        /// Factory method that creates a SpanString from a full string. The resulting
        /// SpanString is allocated on the heap; use the SpanString1 constructor directly
        /// to allocate as a value type.
        /// </summary>
        /// <param name="str">The string whose memory is reused for the SpanString.</param>
        /// <returns>A boxed <see cref="SpanString1"/>.</returns>
        public static ISpanString Create(string str)
        {
            return new SpanString1(str);
        }

        /// <summary>
        /// Factory method that creates a SpanString from two full strings. The resulting
        /// SpanString is allocated on the heap; use the SpanString2 constructor directly
        /// to allocate as a value type.
        /// </summary>
        /// <param name="str1">The first string whose memory is reused for the SpanString.</param>
        /// <param name="str2">The second string whose memory is reused for the SpanString.</param>
        /// <returns>A boxed <see cref="SpanString2"/>.</returns>
        public static ISpanString Create(string str1, string str2)
        {
            return new SpanString2(str1, str2);
        }

        /// <summary>
        /// Factory method that creates a SpanString from zero or more full strings. The resulting
        /// SpanString is allocated on the heap.
        /// </summary>
        /// <param name="strs">The array of full strings used to compose the SpanString.</param>
        /// <returns>A boxed instance of the appropriate SpanStringX struct.</returns>
        /// <exception cref="ArgumentException">
        /// The number of strings in <paramref name="strs"/> exceeds that which can be modeled
        /// by the current SpanString library.
        /// </exception>
        public static ISpanString Create(params string[] strs)
        {
            if (strs == null)
            {
                return Empty;
            }

            if (strs.Length == 1)
            {
                return new SpanString1(strs[0]);
            }

            if (strs.Length == 2)
            {
                return new SpanString2(strs[0], strs[1]);
            }

            throw new ArgumentException($"SpanStrings are currently only available with up to 2 segments");
        }
    }

    /// <summary>
    /// A view onto an underlying <see cref="String"/> that uses Span and Memory to avoid
    /// creating a new String instance.
    /// </summary>
    public struct SpanString1 : ISpanString, IComparable<SpanString1>, IEquatable<SpanString1>
    {
        internal struct Enumerator
        {
            private int _currentIndex;
            private readonly SpanString1 _spanString;

            public Enumerator(SpanString1 ss)
            {
                _currentIndex = 0;
                _spanString = ss;
            }

            // Advances the enumerator, returning the next char value, or -1 if the enumerator is past the end of the string.
            public int Next()
            {
                if (_currentIndex < _spanString._segment.Length)
                {
                    return _spanString._segment.Span[_currentIndex++];
                }

                return -1;
            }
        }

        private readonly ReadOnlyMemory<char> _segment;

        /// <summary>
        /// Initializes an instance of <see cref="SpanString"/> from an entire existing string.
        /// </summary>
        /// <param name="str">The String on which to create a view.</param>
        /// <returns>A <see cref="SpanString"/> instance.</returns>
        public SpanString1(string str)
        {
            _segment = str.AsMemory();
        }

        /// <summary>
        /// Initializes an instance of <see cref="SpanString"/> from the tail of an existing string.
        /// </summary>
        /// <param name="str">The String on which to create a view.</param>
        /// <param name="startIndex">The start index of the span within the string.</param>
        /// <returns>A <see cref="SpanString"/> instance.</returns>
        public SpanString1(string str, int startIndex)
        {
            _segment = str.AsMemory(startIndex);
        }

        /// <summary>
        /// Initializes an instance of <see cref="SpanString"/> from a segment of an existing string.
        /// </summary>
        /// <param name="str">The String on which to create a view.</param>
        /// <param name="startIndex">The start index of the span within the string.</param>
        /// <param name="length">The length of the string segment.</param>
        /// <returns>A <see cref="SpanString"/> instance.</returns>
        public SpanString1(string str, int startIndex, int length)
        {
            _segment = str.AsMemory(startIndex, length);
        }

        /// <inheritdoc/>
        public int Length => _segment.Length;

        /// <inheritdoc/>
        public bool IsWhiteSpace() => _segment.Span.IsWhiteSpace();

        /// <summary>
        /// Compares this SpanString with an object, using an ordinal comparison if the object is a SpanString.
        /// </summary>
        /// <param name="obj">The other object or string to compare with this string.</param>
        /// <returns>True if the strings are equal.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (!(obj is ISpanString ss))
            {
                return false;
            }

            return SpanStringComparer.Ordinal.Equals(this, ss);
        }

        /// <summary>
        /// Compares this SpanString with another SpanString using an ordinal comparison.
        /// </summary>
        /// <param name="other">The other string to compare with this string.</param>
        /// <returns>True if the strings are equal.</returns>
        public bool Equals(SpanString1 other)
        {
            return Equals(this, other);
        }

        /// <summary>
        /// Compares two SpanStrings using an ordinal comparison.
        /// </summary>
        /// <param name="a">The first string to compare.</param>
        /// <param name="b">The second string to compare.</param>
        /// <returns>True if the strings are equal.</returns>
        public static bool Equals(SpanString1 a, SpanString1 b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a._segment.Length; i++)
            {
                if (a._segment.Span[i] != b._segment.Span[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares two SpanStrings using an ordinal comparison.
        /// </summary>
        /// <param name="a">The first string to compare.</param>
        /// <param name="b">The second string to compare.</param>
        /// <returns>True if the strings are equal.</returns>
        public static bool Equals(SpanString1 a, SpanString2 b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            var s1Enum = new Enumerator(a);
            var s2Enum = new SpanString2.Enumerator(b);

            for (int i = 0; i < a.Length; i++)
            {
                int c1 = s1Enum.Next();
                int c2 = s2Enum.Next();
                if (c1 != c2)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares two SpanStrings using an ordinal comparison.
        /// </summary>
        /// <param name="a">The first string to compare.</param>
        /// <param name="b">The second string to compare.</param>
        /// <returns>True if the strings are equal.</returns>
        public static bool EqualsIgnoreCase(SpanString1 a, SpanString1 b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a._segment.Length; i++)
            {
                if (!SpanStringComparer.CharsEqualOrdinalIgnoreCase(a._segment.Span[i], b._segment.Span[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares two SpanStrings using an ordinal comparison.
        /// </summary>
        /// <param name="a">The first string to compare.</param>
        /// <param name="b">The second string to compare.</param>
        /// <returns>True if the strings are equal.</returns>
        public static bool EqualsIgnoreCase(SpanString1 a, SpanString2 b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            var s1Enum = new Enumerator(a);
            var s2Enum = new SpanString2.Enumerator(b);

            for (int i = 0; i < a.Length; i++)
            {
                int c1 = SpanStringComparer.ToUpperAscii(s1Enum.Next());
                int c2 = SpanStringComparer.ToUpperAscii(s2Enum.Next());
                if (c1 != c2)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares two SpanStrings using an ordinal comparison.
        /// </summary>
        /// <param name="a">The first string to compare.</param>
        /// <param name="b">The second string to compare.</param>
        /// <returns>True if the strings are equal.</returns>
        public static bool operator ==(SpanString1 a, SpanString1 b)
        {
            return Equals(a, b);
        }

        /// <summary>
        /// Compares two SpanStrings using an ordinal comparison.
        /// </summary>
        /// <param name="a">The first string to compare.</param>
        /// <param name="b">The second string to compare.</param>
        /// <returns>True if the strings are not equal.</returns>
        public static bool operator !=(SpanString1 a, SpanString1 b)
        {
            return !Equals(a, b);
        }

        /// <summary>
        /// Compares this instance with a specified SpanString1 using an ordinal comparison,
        /// and indicates whether this instance precedes, follows, or appears in the same
        /// position in the sort order as the specified string.
        /// </summary>
        /// <param name="other">The string to compare with this instance.</param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the strB parameter.</returns>
        [Pure]
        public int CompareTo(SpanString1 other) => CompareTo(other, false);

        /// <summary>
        /// Compares this instance with a specified SpanString1 with optional case insensitivity,
        /// and indicates whether this instance precedes, follows, or appears in the same position
        /// in the sort order as the specified string.
        /// </summary>
        /// <param name="other">The string to compare with this instance.</param>
        /// <param name="ignoreCase">Whether to use a case-insensitive comparison.</param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the 'other' parameter.</returns>
        [Pure]
        public int CompareTo(SpanString1 other, bool ignoreCase)
        {
            return _segment.Span.CompareTo(other._segment.Span, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        /// <summary>
        /// Gets a hash code for this string. If strings A and B are such that A.Equals(B), then
        /// they will return the same hash code.
        /// </summary>
        [Pure]
        public override int GetHashCode()
        {
            // Adapted from https://referencesource.microsoft.com/#mscorlib/system/string.cs
            int hash1 = 5381;
            int hash2 = hash1;

            for (int i = 0; i < _segment.Length; i++)
            {
                char c = _segment.Span[i];
                int lowByte = c & 0xFF;
                hash1 = ((hash1 << 5) + hash1) ^ lowByte;
                int highByte = (c & 0xFF00) >> 8;
                hash2 = ((hash2 << 5) + hash2) ^ highByte;
            }

            return hash1 + (hash2 * 1566083941);
        }

        /// <inheritdoc/>
        [Pure]
        public int GetHashCodeIgnoreCase()
        {
            // Adapted from https://referencesource.microsoft.com/#mscorlib/system/string.cs
            int hash1 = 5381;
            int hash2 = hash1;

            for (int i = 0; i < _segment.Length; i++)
            {
                char c = SpanStringComparer.ToUpperAscii(_segment.Span[i]);
                int lowByte = c & 0xFF;
                hash1 = ((hash1 << 5) + hash1) ^ lowByte;
                int highByte = (c & 0xFF00) >> 8;
                hash2 = ((hash2 << 5) + hash2) ^ highByte;
            }

            return hash1 + (hash2 * 1566083941);
        }
    }

    /// <summary>
    /// A view onto two underlying <see cref="String"/> instances that uses Span and Memory
    /// to avoid creating a new String instance.
    /// </summary>
    public struct SpanString2 : ISpanString, IComparable<SpanString2>, IEquatable<SpanString2>
    {
        internal struct Enumerator
        {
            private int _currentSegmentPosition;
            private ReadOnlyMemory<char> _currentSegment;
            private int _currentSegmentIndex;
            private readonly SpanString2 _spanString;

            public Enumerator(SpanString2 ss)
            {
                _currentSegmentPosition = 0;
                _currentSegment = ss._segment1;
                _currentSegmentIndex = 0;
                _spanString = ss;
            }

            // Advances the enumerator, returning the next char value, or -1 if the enumerator is past the end of the string.
            public int Next()
            {
                if (_currentSegmentPosition >= _currentSegment.Length)
                {
                    // Attempt to advance to the next segment.
                    if (_currentSegmentIndex == 0)
                    {
                        _currentSegment = _spanString._segment2;
                        _currentSegmentIndex = 0;
                    }
                    else
                    {
                        return -1;
                    }
                }

                if (_currentSegmentPosition < _currentSegment.Length)
                {
                    return _currentSegment.Span[_currentSegmentPosition++];
                }

                return -1;
            }
        }

        private readonly ReadOnlyMemory<char> _segment1;
        private readonly ReadOnlyMemory<char> _segment2;

        /// <summary>
        /// Initializes a SpanString from two existing strings, treating the different
        /// strings as one contiguous string but without allocating new heap memory for
        /// a single string.
        /// </summary>
        /// <param name="str1">The first string on which to create a view.</param>
        /// <param name="str2">The second string on which to create a view.</param>
        public SpanString2(string str1, string str2)
        {
            _segment1 = str1.AsMemory();
            _segment2 = str2.AsMemory();
        }

        /// <inheritdoc/>
        public int Length => _segment1.Length + _segment2.Length;

        /// <inheritdoc/>
        public bool IsWhiteSpace() => _segment1.Span.IsWhiteSpace() && _segment2.Span.IsWhiteSpace();

        /// <summary>
        /// Compares this SpanString with an object, using an ordinal comparison if the object is a SpanString.
        /// </summary>
        /// <param name="obj">The other object or string to compare with this string.</param>
        /// <returns>True if the strings are equal.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (!(obj is ISpanString ss))
            {
                return false;
            }

            return SpanStringComparer.Ordinal.Equals(this, ss);
        }

        /// <summary>
        /// Compares this SpanString with another SpanString using an ordinal comparison.
        /// </summary>
        /// <param name="other">The other string to compare with this string.</param>
        /// <returns>True if the strings are equal.</returns>
        public bool Equals(SpanString2 other)
        {
            return Equals(this, other);
        }

        /// <summary>
        /// Compares two SpanStrings using an ordinal comparison.
        /// </summary>
        /// <param name="a">The first string to compare.</param>
        /// <param name="b">The second string to compare.</param>
        /// <returns>True if the strings are equal.</returns>
        public static bool Equals(SpanString2 a, SpanString2 b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            var s1Enum = new Enumerator(a);
            var s2Enum = new Enumerator(b);

            for (int i = 0; i < a.Length; i++)
            {
                int c1 = s1Enum.Next();
                int c2 = s2Enum.Next();
                if (c1 != c2)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares two SpanStrings using an ordinal comparison.
        /// </summary>
        /// <param name="a">The first string to compare.</param>
        /// <param name="b">The second string to compare.</param>
        /// <returns>True if the strings are equal.</returns>
        public static bool Equals(SpanString2 a, SpanString1 b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            var s1Enum = new Enumerator(a);
            var s2Enum = new SpanString1.Enumerator(b);

            for (int i = 0; i < a.Length; i++)
            {
                int c1 = s1Enum.Next();
                int c2 = s2Enum.Next();
                if (c1 != c2)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares two SpanStrings using an ordinal comparison.
        /// </summary>
        /// <param name="a">The first string to compare.</param>
        /// <param name="b">The second string to compare.</param>
        /// <returns>True if the strings are equal.</returns>
        public static bool EqualsIgnoreCase(SpanString2 a, SpanString2 b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            var s1Enum = new Enumerator(a);
            var s2Enum = new Enumerator(b);

            for (int i = 0; i < a.Length; i++)
            {
                int c1 = SpanStringComparer.ToUpperAscii(s1Enum.Next());
                int c2 = SpanStringComparer.ToUpperAscii(s2Enum.Next());
                if (c1 != c2)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares two SpanStrings using an ordinal comparison.
        /// </summary>
        /// <param name="a">The first string to compare.</param>
        /// <param name="b">The second string to compare.</param>
        /// <returns>True if the strings are equal.</returns>
        public static bool EqualsIgnoreCase(SpanString2 a, SpanString1 b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            var s1Enum = new Enumerator(a);
            var s2Enum = new SpanString1.Enumerator(b);

            for (int i = 0; i < a.Length; i++)
            {
                int c1 = SpanStringComparer.ToUpperAscii(s1Enum.Next());
                int c2 = SpanStringComparer.ToUpperAscii(s2Enum.Next());
                if (c1 != c2)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares two SpanStrings using an ordinal comparison.
        /// </summary>
        /// <param name="a">The first string to compare.</param>
        /// <param name="b">The second string to compare.</param>
        /// <returns>True if the strings are equal.</returns>
        public static bool operator ==(SpanString2 a, SpanString2 b)
        {
            return Equals(a, b);
        }

        /// <summary>
        /// Compares two SpanStrings using an ordinal comparison.
        /// </summary>
        /// <param name="a">The first string to compare.</param>
        /// <param name="b">The second string to compare.</param>
        /// <returns>True if the strings are not equal.</returns>
        public static bool operator !=(SpanString2 a, SpanString2 b)
        {
            return !Equals(a, b);
        }

        /// <summary>
        /// Compares this instance with a specified SpanString1 using an ordinal comparison,
        /// and indicates whether this instance precedes, follows, or appears in the same
        /// position in the sort order as the specified string.
        /// </summary>
        /// <param name="other">The string to compare with this instance.</param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the strB parameter.</returns>
        public int CompareTo(SpanString1 other)
        {
            // Adapted from https://referencesource.microsoft.com/#mscorlib/system/string.cs CompareOrdinalHelper()

            int length = Math.Min(Length, other.Length);

            var s1Enum = new Enumerator(this);
            var s2Enum = new SpanString1.Enumerator(other);

            for (int i = 0; i < length; i++)
            {
                int c1 = s1Enum.Next();
                int c2 = s2Enum.Next();
                int cmp = c1.CompareTo(c2);
                if (cmp != 0)
                {
                    return cmp;
                }
            }

            // At this point, we have compared all the characters in at least one string.
            // The longer string will be larger.
            return Length - other.Length;
        }

        /// <summary>
        /// Compares this instance with a specified SpanString2 using an ordinal comparison,
        /// and indicates whether this instance precedes, follows, or appears in the same
        /// position in the sort order as the specified string.
        /// </summary>
        /// <param name="other">The string to compare with this instance.</param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the strB parameter.</returns>
        public int CompareTo(SpanString2 other)
        {
            // Adapted from https://referencesource.microsoft.com/#mscorlib/system/string.cs CompareOrdinalHelper()

            int length = Math.Min(Length, other.Length);

            var s1Enum = new Enumerator(this);
            var s2Enum = new Enumerator(other);

            for (int i = 0; i < length; i++)
            {
                int c1 = s1Enum.Next();
                int c2 = s2Enum.Next();
                int cmp = c1.CompareTo(c2);
                if (cmp != 0)
                {
                    return cmp;
                }
            }

            // At this point, we have compared all the characters in at least one string.
            // The longer string will be larger.
            return Length - other.Length;
        }

        /// <summary>
        /// Compares this instance with a specified SpanString1 using an ordinal
        /// comparison and case-insensitivity, and indicates whether this
        /// instance precedes, follows, or appears in the same position
        /// in the sort order as the specified string.
        /// </summary>
        /// <param name="other">The string to compare with this instance.</param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the strB parameter.</returns>
        public int CompareToIgnoreCase(SpanString1 other)
        {
            // Adapted from https://referencesource.microsoft.com/#mscorlib/system/string.cs CompareOrdinalHelper()

            int length = Math.Min(Length, other.Length);

            var s1Enum = new Enumerator(this);
            var s2Enum = new SpanString1.Enumerator(other);

            for (int i = 0; i < length; i++)
            {
                int c1 = SpanStringComparer.ToUpperAscii(s1Enum.Next());
                int c2 = SpanStringComparer.ToUpperAscii(s2Enum.Next());
                int cmp = c1.CompareTo(c2);
                if (cmp != 0)
                {
                    return cmp;
                }
            }

            // At this point, we have compared all the characters in at least one string.
            // The longer string will be larger.
            return Length - other.Length;
        }

        /// <summary>
        /// Compares this instance with a specified SpanString2 using a case insensitive
        /// ordinal comparison, and indicates whether this instance precedes, follows, or
        /// appears in the same position in the sort order as the specified string.
        /// </summary>
        /// <param name="other">The string to compare with this instance.</param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the strB parameter.</returns>
        public int CompareToIgnoreCase(SpanString2 other)
        {
            int length = Math.Min(Length, other.Length);

            var s1Enum = new Enumerator(this);
            var s2Enum = new Enumerator(other);

            for (int i = 0; i < length; i++)
            {
                int c1 = SpanStringComparer.ToUpperAscii(s1Enum.Next());
                int c2 = SpanStringComparer.ToUpperAscii(s2Enum.Next());
                int cmp = c1.CompareTo(c2);
                if (cmp != 0)
                {
                    return cmp;
                }
            }

            // At this point, we have compared all the characters in at least one string.
            // The longer string will be larger.
            return Length - other.Length;
        }

        /// <summary>
        /// Gets a hash code for this string. If strings A and B are such that A.Equals(B), then
        /// they will return the same hash code.
        /// </summary>
        public override int GetHashCode()
        {
            // Adapted from https://referencesource.microsoft.com/#mscorlib/system/string.cs
            int hash1 = 5381;
            int hash2 = hash1;

            for (int i = 0; i < _segment1.Length; i++)
            {
                char c = _segment1.Span[i];
                int lowByte = c & 0xFF;
                hash1 = ((hash1 << 5) + hash1) ^ lowByte;
                int highByte = (c & 0xFF00) >> 8;
                hash2 = ((hash2 << 5) + hash2) ^ highByte;
            }

            for (int i = 0; i < _segment2.Length; i++)
            {
                char c = _segment2.Span[i];
                int lowByte = c & 0xFF;
                hash1 = ((hash1 << 5) + hash1) ^ lowByte;
                int highByte = (c & 0xFF00) >> 8;
                hash2 = ((hash2 << 5) + hash2) ^ highByte;
            }

            return hash1 + (hash2 * 1566083941);
        }

        /// <summary>
        /// Gets a hash code for this string, ignoring case. If strings A and B are such that A.Equals(B), then
        /// they will return the same hash code.
        /// </summary>
        public int GetHashCodeIgnoreCase()
        {
            // Adapted from https://referencesource.microsoft.com/#mscorlib/system/string.cs
            int hash1 = 5381;
            int hash2 = hash1;

            for (int i = 0; i < _segment1.Length; i++)
            {
                char c = SpanStringComparer.ToUpperAscii(_segment1.Span[i]);
                int lowByte = c & 0xFF;
                hash1 = ((hash1 << 5) + hash1) ^ lowByte;
                int highByte = (c & 0xFF00) >> 8;
                hash2 = ((hash2 << 5) + hash2) ^ highByte;
            }

            for (int i = 0; i < _segment2.Length; i++)
            {
                char c = SpanStringComparer.ToUpperAscii(_segment2.Span[i]);
                int lowByte = c & 0xFF;
                hash1 = ((hash1 << 5) + hash1) ^ lowByte;
                int highByte = (c & 0xFF00) >> 8;
                hash2 = ((hash2 << 5) + hash2) ^ highByte;
            }

            return hash1 + (hash2 * 1566083941);
        }
    }

    /// <summary>
    /// Implements case sensitive and insensitive comparer logic for SpanStrings.
    /// </summary>
    public class SpanStringComparer :
        IEqualityComparer<ISpanString>,
        IEqualityComparer<SpanString1>,
        IEqualityComparer<SpanString2>
    {
        private readonly bool _ignoreCase;

        private SpanStringComparer(bool ignoreCase)
        {
            _ignoreCase = ignoreCase;
        }

        /// <summary>
        /// Gets a SpanStringComparer instance that performs ordinal character comparisons.
        /// </summary>
        public static SpanStringComparer Ordinal { get; } = new SpanStringComparer(false);

        /// <summary>
        /// Gets a SpanStringComparer instance that performs case-insensitive ordinal character comparisons.
        /// </summary>
        public static SpanStringComparer OrdinalIgnoreCase { get; } = new SpanStringComparer(true);

        /// <summary>
        /// Compares two strings.
        /// </summary>
        /// <param name="a">The first string to compare.</param>
        /// <param name="b">The second string to compare.</param>
        /// <returns>True if the strings are equal.</returns>
        public bool Equals(SpanString1 a, SpanString1 b)
        {
            return _ignoreCase ? SpanString1.EqualsIgnoreCase(a, b) : a.Equals(b);
        }

        /// <summary>
        /// Compares two strings.
        /// </summary>
        /// <param name="a">The first string to compare.</param>
        /// <param name="b">The second string to compare.</param>
        /// <returns>True if the strings are equal.</returns>
        public bool Equals(SpanString2 a, SpanString2 b)
        {
            return _ignoreCase ? SpanString2.EqualsIgnoreCase(a, b) : a.Equals(b);
        }

        /// <summary>
        /// Compares two strings.
        /// </summary>
        /// <param name="a">The first string to compare.</param>
        /// <param name="b">The second string to compare.</param>
        /// <returns>True if the strings are equal.</returns>
        public bool Equals(ISpanString a, ISpanString b)
        {
            // Find optimized comparison methods.
            if (a is SpanString1 a1)
            {
                if (b is SpanString1 b1)
                {
                    return _ignoreCase ? SpanString1.EqualsIgnoreCase(a1, b1) : SpanString1.Equals(a1, b1);
                }

                if (b is SpanString2 b2)
                {
                    return _ignoreCase ? SpanString1.EqualsIgnoreCase(a1, b2) : SpanString1.Equals(a1, b2);
                }

                throw new ArgumentException($"Second param 'b' is an unknown SpanString type '{b.GetType().Name}', is this a bug?");
            }

            if (a is SpanString2 a2)
            {
                if (b is SpanString1 b1)
                {
                    return _ignoreCase ? SpanString2.EqualsIgnoreCase(a2, b1) : SpanString2.Equals(a2, b1);
                }

                if (b is SpanString2 b2)
                {
                    return _ignoreCase ? SpanString2.EqualsIgnoreCase(a2, b2) : SpanString2.Equals(a2, b2);
                }

                throw new ArgumentException($"Second param 'b' is an unknown SpanString type '{b.GetType().Name}', is this a bug?");
            }

            throw new ArgumentException($"First param 'a' is an unknown SpanString type '{a.GetType().Name}', is this a bug?");
        }

        /// <summary>
        /// Gets a hash code for a SpanString. If strings A and B are such that A.Equals(B), then
        /// they will return the same hash code.
        /// </summary>
        public int GetHashCode(ISpanString s)
        {
            if (!_ignoreCase)
            {
                return s.GetHashCode();
            }

            return s.GetHashCodeIgnoreCase();
        }

        /// <summary>
        /// Gets a hash code for a SpanString. If strings A and B are such that A.Equals(B), then
        /// they will return the same hash code.
        /// </summary>
        public int GetHashCode(SpanString1 s)
        {
            if (!_ignoreCase)
            {
                return s.GetHashCode();
            }

            return s.GetHashCodeIgnoreCase();
        }

        /// <summary>
        /// Gets a hash code for a SpanString. If strings A and B are such that A.Equals(B), then
        /// they will return the same hash code.
        /// </summary>
        public int GetHashCode(SpanString2 s)
        {
            if (!_ignoreCase)
            {
                return s.GetHashCode();
            }

            return s.GetHashCodeIgnoreCase();
        }

        internal static char ToUpperAscii(char c)
        {
            // Adapted from https://referencesource.microsoft.com/#mscorlib/system/string.cs EqualsIgnoreCaseAsciiHelper()
            // TODO: We need to get the slow-path case-insensitive compare logic from
            // .NET Framework's native code comnlsinfo.cpp InternalCompareStringOrdinalIgnoreCase()
            if ((uint)((c | 0x20) - 'a') <= (uint)('z' - 'a'))
            {
                return (char)(c & ~0x20);
            }

            return c;
        }

        internal static int ToUpperAscii(int c)
        {
            // Adapted from https://referencesource.microsoft.com/#mscorlib/system/string.cs EqualsIgnoreCaseAsciiHelper()
            // TODO: We need to get the slow-path case-insensitive compare logic from
            // .NET Framework's native code comnlsinfo.cpp InternalCompareStringOrdinalIgnoreCase()
            if ((uint)((c | 0x20) - 'a') <= (uint)('z' - 'a'))
            {
                return c & ~0x20;
            }

            return c;
        }

        internal static bool CharsEqualOrdinalIgnoreCase(char a, char b)
        {
            // Adapted from https://referencesource.microsoft.com/#mscorlib/system/string.cs EqualsIgnoreCaseAsciiHelper()
            // TODO: We need to get the slow-path case-insensitive compare logic from
            // .NET Framework's native code comnlsinfo.cpp InternalCompareStringOrdinalIgnoreCase()

            // Ordinal equals or lowercase equals if the result ends up in the a-z range 
            if (a == b)
            {
                return true;
            }

            return (a | 0x20) == (b | 0x20) &&
                (uint)((a | 0x20) - 'a') <= (uint)('z' - 'a');
        }
    }
}
