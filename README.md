# FuzzySearch for C#

A lightweight, in-memory fuzzy search utility for C# objects. Search across automatically discovered string properties or select specific fields with strongly typed expressions. Results retain the original item type and are returned in descending relevance order.

```csharp
List<ItemEntity> results = FuzzySearch.Search(
    items,
    "Safty",
    x => x.ItemName);
```

`"Safty"` can match `"Safety Helmet"`: the search compares the query with individual words as well as the complete field value.

> This README describes the supplied `FuzzySearchService.cs` implementation. The public class is named `FuzzySearch`. Proposed enhancements at the end are not implemented features.

## Contents

- [Features](#features)
- [Setup](#setup)
- [Quick start](#quick-start)
- [Usage examples](#usage-examples)
- [API reference](#api-reference)
- [How scoring works](#how-scoring-works)
- [Behavior and edge cases](#behavior-and-edge-cases)
- [Performance](#performance)
- [Code review and recommended enhancements](#code-review-and-recommended-enhancements)
- [Suggested tests](#suggested-tests)

## Features

- **Automatic field discovery:** searches public instance string properties without requiring a field list.
- **Explicit field selection:** searches one or more fields using expressions such as `x => x.ItemName`.
- **Typo tolerance:** uses normalized Levenshtein similarity for insertions, deletions, and substitutions.
- **Multiple matching rules:** supports exact, prefix, substring, whole-field similarity, and word similarity matching.
- **Case-insensitive matching:** trims text and applies invariant lowercase normalization.
- **Configurable threshold:** defaults to `0.70` and includes scores equal to the threshold.
- **Typed results:** accepts `IEnumerable<T>` and returns `List<T>` containing the original items.
- **Cached reflection metadata:** caches discovered properties by runtime type in a `ConcurrentDictionary`.
- **No third-party dependencies:** the supplied implementation uses .NET framework libraries only.

## Setup

Add [FuzzySearchService.cs](FuzzySearchService.cs) to your project or to a class library referenced by your application. Adjust this relative link if the source lives in a subdirectory.

The source uses APIs available in .NET 6 and later, including `ArgumentNullException.ThrowIfNull`, and C# 9 syntax such as target-typed `new()`. Use a compatible target framework and language version. This is a source compatibility observation, not a verified framework support matrix.

Enable nullable annotations to work with the `string?` declarations:

```xml
<PropertyGroup>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
</PropertyGroup>
```

If your project does not supply implicit or global usings, add these imports to the source file alongside its existing imports:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
```

The supplied class has no namespace. You can place it in your library's namespace and import that namespace in consuming projects.

## Quick start

The following example assumes `FuzzySearchService.cs` is included in a console project with implicit usings enabled:

```csharp
var items = new List<ItemEntity>
{
    new() { ItemId = 1, ItemName = "Safety Helmet", Category = "PPE" },
    new() { ItemId = 2, ItemName = "Safety Gloves", Category = "PPE" },
    new() { ItemId = 3, ItemName = "Hammer", Category = "Tools" }
};

List<ItemEntity> matches = FuzzySearch.Search(
    items,
    "Safty",
    x => x.ItemName);

foreach (ItemEntity item in matches)
{
    Console.WriteLine($"{item.ItemId}: {item.ItemName}");
}

public sealed class ItemEntity
{
    public long ItemId { get; set; }
    public string? ItemName { get; set; }
    public string? Manufacturer { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public string? PartNumber { get; set; }
    public int Quantity { get; set; }
}
```

Expected output based on the implementation:

```text
1: Safety Helmet
2: Safety Gloves
```

Both names receive a word similarity score of approximately `0.8333`, exceeding the default threshold. Equal scores preserve source order.

## Usage examples

### Search all string properties

```csharp
List<ItemEntity> matches = FuzzySearch.Search(items, "Safty");
```

Automatic discovery includes `ItemName`, `Manufacturer`, `Category`, `Description`, and `PartNumber`. It excludes numeric properties such as `ItemId` and `Quantity`.

### Search one field

```csharp
List<ItemEntity> matches = FuzzySearch.Search(
    items,
    "Safty",
    x => x.ItemName);
```

### Search multiple fields

```csharp
List<ItemEntity> matches = FuzzySearch.Search(
    items,
    "Safty",
    x => x.ItemName,
    x => x.Description,
    x => x.Category);
```

An item qualifies when **any selected field** reaches the threshold. Its final score is the highest field score; scores are not summed or averaged.

### Set a custom threshold

```csharp
// Search every discovered string property.
List<ItemEntity> broadMatches = FuzzySearch.Search(
    items,
    "Safty",
    minimumScore: 0.60);

// Search selected fields with a custom threshold.
List<ItemEntity> focusedMatches = FuzzySearch.Search(
    items,
    "Safty",
    0.80,
    x => x.ItemName,
    x => x.PartNumber);
```

Lower thresholds admit more approximate matches. Higher thresholds reduce them. A score is a matching heuristic, not a probability or confidence percentage.

### Return all items for an empty query

```csharp
List<ItemEntity> allItems = FuzzySearch.Search(items, "");
```

An empty or whitespace query returns a new list in source order without applying scoring or filtering.

### Search a nested value explicitly

Automatic discovery does not traverse nested objects, but selectors can access nested values. For a model with a nullable `Supplier` object:

```csharp
var matches = FuzzySearch.Search(
    products,
    "Acme",
    x => x.Supplier == null ? null : x.Supplier.Name);
```

Selectors are compiled expressions, so they can also return computed strings. The implementation does not require them to be simple property accesses. Handle null nested objects inside the selector.

## API reference

```csharp
public const double DefaultMinimumScore = 0.70;

public static List<T> Search<T>(
    IEnumerable<T> source,
    string searchText,
    double minimumScore = DefaultMinimumScore);

public static List<T> Search<T>(
    IEnumerable<T> source,
    string searchText,
    params Expression<Func<T, string?>>[] properties);

public static List<T> Search<T>(
    IEnumerable<T> source,
    string searchText,
    double minimumScore,
    params Expression<Func<T, string?>>[] properties);
```

| Parameter | Behavior |
| --- | --- |
| `source` | Sequence to materialize and search. A null sequence throws `ArgumentNullException`. |
| `searchText` | Query normalized with `Trim().ToLowerInvariant()`. Null is accepted at runtime despite the non-nullable declaration. |
| `minimumScore` | Inclusive score cutoff; intended range is `0.0`–`1.0`, but the implementation does not validate it. |
| `properties` | Selectors for fields to search. Omitted, empty, null, or all-null selectors fall back to automatic discovery. Null entries in a mixed array are ignored. |
| Return value | A newly allocated `List<T>`. The element type is preserved; the input collection type is not. Reference-type items are not cloned. |

Scores and matched field names are internal and are not included in the public result.

## How scoring works

For each nonempty field, the implementation normalizes its value and evaluates these rules in order:

| Order | Rule | Result |
| --- | --- | --- |
| 1 | Entire field equals the query | Immediately returns `1.00`. |
| 2 | Entire field starts with the query | Immediately returns `0.95`. |
| 3 | Entire field contains the query | Immediately returns `0.90`. |
| 4 | None of the above | Computes whole-field and individual-word similarity, keeping the highest score. |

Levenshtein similarity is calculated as:

```text
similarity = 1 - editDistance / max(fieldOrWordLength, queryLength)
```

For `"safty"` and `"safety"`, the edit distance is `1` and the maximum length is `6`:

```text
similarity = 1 - 1 / 6 = 0.833333...
```

Examples of the current scoring behavior:

| Query | Field value | Score | Reason |
| --- | --- | --- | --- |
| `safety` | `Safety` | `1.00` | Whole-field equality after normalization. |
| `safety` | `Safety Helmet` | `0.95` | Whole-field prefix. |
| `safety` | `Industrial Safety Helmet` | `0.90` | Whole-field substring. |
| `safty` | `Safety Helmet` | Approximately `0.8333` | Similarity to the word `safety`. |

**The matching rules are ordered shortcuts, not a global maximum across every rule.** Prefix and substring matches return before word scoring. Consequently, an exact word within a longer field does not receive `1.00`. The later exact-word and word-prefix branches cannot take effect because those queries already matched the whole-field prefix or substring checks.

At the entity level, the best field score becomes the item score. Matching items are sorted descending, with input order preserved for ties. Duplicate source items are retained.

## Behavior and edge cases

- **Null source:** throws immediately.
- **Blank query:** returns all source items, including null entries, without accessing properties.
- **Nonblank query:** skips null items.
- **Null or blank field:** contributes a score of zero.
- **No string properties:** produces an entity score of zero.
- **Zero or negative threshold:** includes every non-null item, even items with no searchable text.
- **Threshold above one or `double.NaN`:** yields no matches for a nonblank query. Positive infinity also yields no matches; negative infinity includes every non-null item.
- **Getter or selector failure:** exceptions propagate to the caller; they are not suppressed.
- **Discovery scope:** uses each item's runtime type, includes inherited eligible properties, and excludes indexers, fields, static properties, and non-string properties.
- **Getter visibility:** discovery checks `CanRead`, which is not the same as requiring a public getter. A public property with a non-public getter can enter the cache.
- **Normalization:** does not remove accents or punctuation, collapse internal whitespace, or perform Unicode normalization.
- **Word splitting:** splits on the literal space character only. Tabs, newlines, and hyphens do not act as general token separators.
- **Multiword queries:** are compared as one query string; words are not independently required across fields, and word order is not ignored.
- **Transpositions:** adjacent swapped characters are not treated as a special single edit; this is standard Levenshtein, not Damerau–Levenshtein.
- **Strings as items:** automatic discovery is intended for object properties. A `List<string>` needs an explicit selector such as `FuzzySearch.Search(names, "Safty", x => x)`.

## Performance

The method executes synchronously and materializes the source with `ToList()` before searching. Passing a database-backed enumerable can therefore load its entire result set into memory; this is not a database-translatable fuzzy search operation. Filter or otherwise bound external data before calling it.

Reflection property metadata is cached across calls. Property values are still read through reflection for each item in automatic mode. Explicit selectors are compiled once per search invocation, then reused for the items in that invocation; compiled selectors are not cached between searches.

For strings of lengths `m` and `n`, each Levenshtein comparison takes `O(m × n)` time and `O(n)` auxiliary memory using two integer rows. The rows are allocated for each comparison. The implementation may compare both a full field and each of its words.

Returning `k` matches adds sorting work of approximately `O(k log k)`. Long descriptions, many fields, repeated calls, and large collections increase cost. There is no result limit, cancellation support, or built-in debounce.

For an interactive search box, select relevant fields and debounce input. Measure performance against your actual data before using it on a UI thread or over a large dataset. No benchmark results are supplied with this review.

## Code review and recommended enhancements

### Strengths

The public overloads keep common searches concise while allowing type-safe field selection. Separating entity scoring, field scoring, normalization, and distance calculation makes the implementation easy to follow. Metadata caching avoids repeated property discovery, and the two-row distance algorithm avoids a full distance matrix.

### Priority 1: Make the contract consistent

1. **Validate `minimumScore`.** Reject `double.NaN`, infinity, and values outside `[0, 1]` with `ArgumentOutOfRangeException`. Decide whether validation should also run for blank queries; validating before materializing the source avoids unnecessary enumeration for invalid input.
2. **Resolve the scoring shortcuts.** Decide whether exact words should outrank prefixes and substrings. If so, evaluate token rules before returning from substring matching, or compute the maximum across the intended scoring rules. If existing scores are intentional, remove the unreachable word branches and document that policy. Changing this can change result ordering and threshold behavior.
3. **Align nullability with behavior.** Declare `searchText` as `string?` if null intentionally means “return all.” Specify whether blank queries should retain null items, since nonblank queries remove them.
4. **Require public getters during discovery.** Add a check such as `property.GetMethod?.IsPublic == true` to match the documented public-readable-property contract.

### Priority 2: Improve repeated-search efficiency

1. Introduce a reusable search configuration with precompiled selectors for repeated queries. Avoid caching arbitrary expression instances indefinitely.
2. Consider cached accessor delegates for automatic discovery, with an explicit strategy for runtime code generation and deployment compatibility.
3. Avoid the intermediate `source.ToList()` on nonblank searches if a single-pass implementation meets the desired enumeration semantics.
4. Reduce distance-array allocations and use the shorter string for the array dimension. Preserve existing scores with regression tests before adding threshold-based pruning.

### Priority 3: Extend the API deliberately

1. Add a separate scored-result API that exposes the item, score, and matched field while retaining the simple `List<T>` API.
2. Add optional tokenization and normalization policies for punctuation, Unicode, accents, and multiword queries.
3. Consider cancellation and a result limit for interactive or larger searches. A result limit only reduces scoring work if the algorithm is designed to exploit it.
4. Add a library namespace, project metadata, and an explicit repository license before publishing a package. No package identity or license can be inferred from the supplied file.

## Suggested tests

These are proposed regression cases, not a test suite included with the supplied source:

| Case | Expected current behavior |
| --- | --- |
| `" SAFETY "` against `"Safety"` | Score `1.00` after normalization. |
| `"Safty"` against `"Safety Helmet"`, threshold `0.80` | Included. |
| Same typo with threshold `0.85` | Excluded when only that field is searched. |
| `"safety"` against `"Industrial Safety Helmet"`, threshold `0.95` | Excluded because substring matching returns `0.90`. |
| Field score exactly equal to the threshold | Included. |
| Equal scores for two items | Preserve source order. |
| Query matches an unselected property only | Excluded if selected fields remain below the threshold. |
| Empty selector array | Searches all discovered string properties. |
| Null source | Throws `ArgumentNullException`. |
| Blank query with a null item in the source | Retains the null item. |
| Nonblank query with a null item in the source | Skips the null item. |
| No searchable strings and threshold `0` | Includes the non-null item. |
| Negative, greater-than-one, or NaN threshold | Does not throw; follows the comparison behavior documented above. |
| A property getter throws | Search fails and the exception propagates. |

**Review scope:** static inspection of `FuzzySearchService.cs`. The .NET SDK was unavailable in the review environment, so compilation, runtime tests, framework compatibility, and performance have not been verified. Expected outputs and scores are derived from the source.
