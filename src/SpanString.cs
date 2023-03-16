// Copyright (c) Erik Mavrinac, https://github.com/erikma. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace OptimizedStrings;

/// <summary>
/// A factory class for SpanStrings.
/// </summary>
public static class SpanString
{
    /// <summary>
    /// Gets a SpanString containing only the empty string.
    /// </summary>
    /// <remarks>This is a boxed <see cref="SpanString1"/>.</remarks>
    public static ISpanString Empty { get; } = new SpanString1(string.Empty);

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
    public static ISpanString Create(params string[]? strs)
    {
        if (strs is null || strs.Length == 0)
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

        throw new ArgumentException("SpanStrings are currently only available with up to 2 segments");
    }
}
