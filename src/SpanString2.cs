// Copyright (c) Erik Mavrinac, https://github.com/erikma. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace OptimizedStrings;

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
    public override bool Equals(object? obj)
    {
        if (obj is ISpanString ss)
        {
            return SpanStringComparer.Ordinal.Equals(this, ss);
        }

        return false;
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
