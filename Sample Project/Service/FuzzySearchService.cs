using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

public static class FuzzySearch
{
    public const double DefaultMinimumScore = 0.70;

    private static readonly ConcurrentDictionary<Type, PropertyInfo[]>
        StringPropertyCache = new();

    /// <summary>
    /// Performs fuzzy matching against all readable public string properties of each item
    /// in the provided source collection and returns matching items ordered by relevance.
    /// </summary>
    /// <typeparam name="T">
    /// The type of items to search.
    /// </typeparam>
    /// <param name="source">
    /// The input sequence of items to evaluate.
    /// </param>
    /// <param name="searchText">
    /// The text to match against string properties. The value is normalized
    /// (trimmed and converted to lowercase) before scoring.
    /// </param>
    /// <param name="minimumScore">
    /// The minimum similarity score required for an item to be included in the result.
    /// Valid values are between 0.0 and 1.0. Default is <see cref="DefaultMinimumScore"/>.
    /// </param>
    /// <returns>
    /// A list of matching items sorted in descending score order.
    /// If <paramref name="searchText"/> is null, empty, or whitespace, all source items are returned.
    /// </returns>
    public static List<T> Search<T>(
        IEnumerable<T> source,
        string searchText,
        double minimumScore = DefaultMinimumScore)
    {
        return SearchInternal(
            source,
            searchText,
            minimumScore,
            null);
    }

    /// <summary>
    /// Performs fuzzy matching against one or more selected string properties from each item
    /// in the provided source collection and returns matching items ordered by relevance.
    /// </summary>
    /// <typeparam name="T">
    /// The type of items to search.
    /// </typeparam>
    /// <param name="source">
    /// The input sequence of items to evaluate.
    /// </param>
    /// <param name="searchText">
    /// The text to match against the selected properties. The value is normalized
    /// (trimmed and converted to lowercase) before scoring.
    /// </param>
    /// <param name="properties">
    /// One or more expressions that select string properties to search.
    /// If no expressions are provided, all readable public string properties are searched.
    /// </param>
    /// <returns>
    /// A list of matching items sorted in descending score order using
    /// <see cref="DefaultMinimumScore"/> as the threshold.
    /// If <paramref name="searchText"/> is null, empty, or whitespace, all source items are returned.
    /// </returns>
    public static List<T> Search<T>(
        IEnumerable<T> source,
        string searchText,
        params Expression<Func<T, string?>>[] properties)
    {
        return SearchInternal(
            source,
            searchText,
            DefaultMinimumScore,
            properties);
    }

    /// <summary>
    /// Performs fuzzy matching against one or more selected string properties from each item
    /// in the provided source collection and returns matching items ordered by relevance.
    /// </summary>
    /// <typeparam name="T">
    /// The type of items to search.
    /// </typeparam>
    /// <param name="source">
    /// The input sequence of items to evaluate.
    /// </param>
    /// <param name="searchText">
    /// The text to match against the selected properties. The value is normalized
    /// (trimmed and converted to lowercase) before scoring.
    /// </param>
    /// <param name="minimumScore">
    /// The minimum similarity score required for an item to be included in the result.
    /// Valid values are between 0.0 and 1.0.
    /// </param>
    /// <param name="properties">
    /// One or more expressions that select string properties to search.
    /// If no expressions are provided, all readable public string properties are searched.
    /// </param>
    /// <returns>
    /// A list of matching items sorted in descending score order.
    /// If <paramref name="searchText"/> is null, empty, or whitespace, all source items are returned.
    /// </returns>
    public static List<T> Search<T>(
        IEnumerable<T> source,
        string searchText,
        double minimumScore,
        params Expression<Func<T, string?>>[] properties)
    {
        return SearchInternal(
            source,
            searchText,
            minimumScore,
            properties);
    }

    private static List<T> SearchInternal<T>(
        IEnumerable<T> source,
        string searchText,
        double minimumScore,
        Expression<Func<T, string?>>[]? propertyExpressions)
    {
        ArgumentNullException.ThrowIfNull(source);

        List<T> sourceList = source.ToList();

        if (string.IsNullOrWhiteSpace(searchText))
            return sourceList;

        searchText = Normalize(searchText);

        // If properties were explicitly provided,
        // compile their getters once.
        Func<T, string?>[]? propertyGetters = propertyExpressions?
            .Where(x => x != null)
            .Select(x => x.Compile())
            .ToArray();

        bool searchAllProperties =
            propertyGetters == null ||
            propertyGetters.Length == 0;

        return sourceList
            .Where(item => item != null)
            .Select(item => new
            {
                Item = item!,
                Score = searchAllProperties
                    ? CalculateEntityScore(
                        item!,
                        searchText)
                    : CalculateSelectedPropertiesScore(
                        item!,
                        searchText,
                        propertyGetters!)
            })
            .Where(x => x.Score >= minimumScore)
            .OrderByDescending(x => x.Score)
            .Select(x => x.Item)
            .ToList();
    }

    private static double CalculateSelectedPropertiesScore<T>(
        T item,
        string searchText,
        Func<T, string?>[] propertyGetters)
    {
        double bestScore = 0.0;

        foreach (Func<T, string?> getter in propertyGetters)
        {
            string? value = getter(item);

            double score = CalculatePropertyScore(
                value,
                searchText);

            if (score > bestScore)
                bestScore = score;

            if (bestScore >= 1.0)
                break;
        }

        return bestScore;
    }

    private static double CalculateEntityScore<T>(
        T item,
        string searchText)
    {
        PropertyInfo[] properties = GetStringProperties(
            item!.GetType());

        double bestScore = 0.0;

        foreach (PropertyInfo property in properties)
        {
            string? value = property.GetValue(item) as string;

            double score = CalculatePropertyScore(
                value,
                searchText);

            if (score > bestScore)
                bestScore = score;

            if (bestScore >= 1.0)
                break;
        }

        return bestScore;
    }

    private static double CalculatePropertyScore(
        string? value,
        string searchText)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0;

        value = Normalize(value);

        // Exact match
        if (value == searchText)
            return 1.0;

        // Starts with
        if (value.StartsWith(
            searchText,
            StringComparison.Ordinal))
        {
            return 0.95;
        }

        // Contains
        if (value.Contains(
            searchText,
            StringComparison.Ordinal))
        {
            return 0.90;
        }

        double bestScore = GetSimilarity(
            value,
            searchText);

        // Compare each word individually
        string[] words = value.Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries |
            StringSplitOptions.TrimEntries);

        foreach (string word in words)
        {
            if (word == searchText)
                return 1.0;

            if (word.StartsWith(
                searchText,
                StringComparison.Ordinal))
            {
                bestScore = Math.Max(
                    bestScore,
                    0.95);

                continue;
            }

            double score = GetSimilarity(
                word,
                searchText);

            if (score > bestScore)
                bestScore = score;
        }

        return bestScore;
    }

    private static PropertyInfo[] GetStringProperties(
        Type type)
    {
        return StringPropertyCache.GetOrAdd(
            type,
            entityType => entityType
                .GetProperties(
                    BindingFlags.Public |
                    BindingFlags.Instance)
                .Where(property =>
                    property.PropertyType == typeof(string) &&
                    property.CanRead &&
                    property.GetIndexParameters().Length == 0)
                .ToArray());
    }

    private static double GetSimilarity(
        string source,
        string target)
    {
        if (source == target)
            return 1.0;

        if (string.IsNullOrEmpty(source) ||
            string.IsNullOrEmpty(target))
        {
            return 0;
        }

        int distance = LevenshteinDistance(
            source,
            target);

        int maxLength = Math.Max(
            source.Length,
            target.Length);

        return 1.0 -
               ((double)distance / maxLength);
    }

    private static int LevenshteinDistance(
        string source,
        string target)
    {
        if (source.Length == 0)
            return target.Length;

        if (target.Length == 0)
            return source.Length;

        int[] previous =
            new int[target.Length + 1];

        int[] current =
            new int[target.Length + 1];

        for (int j = 0; j <= target.Length; j++)
            previous[j] = j;

        for (int i = 1; i <= source.Length; i++)
        {
            current[0] = i;

            for (int j = 1; j <= target.Length; j++)
            {
                int cost =
                    source[i - 1] == target[j - 1]
                        ? 0
                        : 1;

                current[j] = Math.Min(
                    Math.Min(
                        current[j - 1] + 1,
                        previous[j] + 1),
                    previous[j - 1] + cost);
            }

            (previous, current) =
                (current, previous);
        }

        return previous[target.Length];
    }

    private static string Normalize(string value)
    {
        return value
            .Trim()
            .ToLowerInvariant();
    }
}