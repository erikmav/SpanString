// Copyright (c) Erik Mavrinac, https://github.com/erikma. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace OptimizedStrings
{
    /// <summary>
    /// The properties and methods exposed by SpanStrings.
    /// </summary>
    public interface ISpanString
    {
        /// <summary>
        /// Gets the length of the SpanString in characters.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Indicates whether this SpanString is empty or composed only of white-space
        /// characters.
        /// </summary>
        /// <returns>True if the SpanString is empty or contains only white-space characters.</returns>
        bool IsWhiteSpace();

        /// <summary>
        /// Gets a hash code for this string, ignoring case. If strings A and B are such that A.Equals(B), then
        /// they will return the same hash code.
        /// </summary>
        int GetHashCodeIgnoreCase();
    }
}
