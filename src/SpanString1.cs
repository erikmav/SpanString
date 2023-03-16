// Copyright (c) Erik Mavrinac, https://github.com/erikma. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Contracts;

namespace OptimizedStrings;

/// <summary>
/// A view onto all or a slice of an underlying <see cref="String"/> that uses Span and Memory to avoid
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
