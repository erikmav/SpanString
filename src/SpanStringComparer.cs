// Copyright (c) Erik Mavrinac, https://github.com/erikma. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace OptimizedStrings;

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
    public bool Equals(ISpanString? a, ISpanString? b)
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

            throw new ArgumentException($"Second param 'b' is an unknown SpanString type '{b?.GetType().Name}', is this a bug?");
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

            throw new ArgumentException($"Second param 'b' is an unknown SpanString type '{b?.GetType().Name}', is this a bug?");
        }

        throw new ArgumentException($"First param 'a' is an unknown SpanString type '{a?.GetType().Name}', is this a bug?");
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
