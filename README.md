# SpanString
SpanString is an optimized string data structure built from `Memory<char>` and `Span<char>` added in .NET Standard 2.1+ and .NET Core 3.0+. It allows you to gather string segments from the memory of other strings to act as a single string. This can be useful for applications where millions of strings are parsed and used as keys for dictionaries or other data structures, and memory pressure and GC overhead are important concerns.

SpanString always uses ordinal string comparisons, and currently only handles ANSI for OrdinalIgnoreCase comparisons.

The release output of this repo generates the [SpanString](https://www.nuget.org/packages/SpanString/) package on Nuget.org.

## Release Notes

### 0.1.0 22 Apr 2019
Initial release for testing performance and memory use versus strings in cases of reading and parsing millions of strings and creating substrings. Only contains SpanString1,2 allowing up to only 2 string segments to compose a SpanString.
