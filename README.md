# SpanString
SpanString is an optimized string data structure built from `Memory<char>` and `Span<char>` added in .NET Standard 2.1+ and .NET Core 3.0+. It allows you to gather string segments from the memory of other strings to act as a single string. This can be useful for applications where millions of strings are parsed and used as keys for dictionaries or other data structures, and memory pressure and GC overhead are important concerns.

SpanString always uses ordinal string comparisons, and currently only handles ANSI for OrdinalIgnoreCase comparisons.

The release output of this repo generates the [SpanString](https://www.nuget.org/packages/SpanString/) package on Nuget.org.

## Usage
SpanStrings are exposed in the `OptimizedStrings` namespace.

### SpanString
The `SpanString` class acts as a factory for various construction patterns, returning `ISpanString` instances. These instances are boxed versions of the SpanString1, 2, ... classes noted below.

### SpanStringComparer
Provides various static helpers like `Ordinal` and `OrdinalIgnoreCase` for use as the comparers for SpanStrings used in hashmap-type collections.

### SpanString1 struct
A `SpanString1` wraps all or part of an underlying string object with a `ReadOnlyMemory<char>` to allow slicing out a subset of a string. The SpanString1 keeps a ref to the underlying string while letting you use it for a subset of string operations. It is a struct to let you control memory placement and avoid heap allocation overhead; if you want a boxed version allocated on the heap, use `SpanString.Create` to get an ISpanString that boxes this struct.

Use cases: One typical case for this is when parsing paths and you want to use one long path as the basis for multiple subpaths, all kept as references to parts of the long path string. `SpanString1` implements IEquatable and can act as its own default ordinally-compared key in a Dictionary, ConcurrentDictionary, HashSet, or other hashmap-like collection, or use `SpanStringComparer.OrdinalIgnoreCase` as the comparer to get case-insensitive keys.

```
using OptimizedStrings;

var windowsRelativePaths = new Dictionary<SpanString1, DirectoryInfo>(SpanStringComparer.OrdinalIgnoreCase);

string aPath = @"c:\windows\Microsoft.NET\Framework64\v4.0.30319\System.Web.Services.dll";

// No need to do aPath.Substring(11) to create a
// new string after 'c:\windows\' - just slice it!
var ss = new SpanString1(aPath, 11);
windowsRelativePaths[ss] = new DirectoryInfo(aPath);
```

### SpanString2 struct
A `SpanString2` wraps two underlying strings and treats them as one for comparison, equality, hash code, and other calculations. It keeps two refs to the underlying strings' memory. The two strings can actually be from the same string. It is a struct to let you control memory placement and avoid heap allocation overhead; if you want a boxed version allocated on the heap, use `SpanString.Create` to get an ISpanString that boxes this struct.

Use cases: Creating a multi-part string key from two individual parts becomes more efficient when you don't need to allocate a third string that concatenates the two parts together. Instead use a SpanString2 to wrap both parts and use as a key in a Dictionary, ConcurrentDictionary, HashSet, and so on. `SpanString2` implements IEquatable and can act as its own default ordinally-compared key in a Dictionary, HashSet, or other hashmap-like collection. Or use `SpanStringComparer.OrdinalIgnoreCase` as the comparer to get case-insensitive keys.

```
using OptimizedStrings;

class MyDatabaseRecord
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }

    public SpanString2 ToKey()
    {
        return new SpanString2(FirstName, LastName);
    }
}

var table = new Dictionary<SpanString2, DirectoryInfo>(SpanStringComparer.OrdinalIgnoreCase);

foreach (MyDatabaseRecord record in database.GetRecords())
{
    table[record.ToKey()] = record;
}
```

## Release Notes

### 0.1.0 22 Apr 2019
Initial release for testing performance and memory use versus strings in cases of reading and parsing millions of strings and creating substrings. Only contains SpanString1,2 allowing up to only 2 string segments to compose a SpanString.
