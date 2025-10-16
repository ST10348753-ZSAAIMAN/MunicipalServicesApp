using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// Records search behaviour for recommendations:
    ///   - Stack<string>     search history (LIFO)
    ///   - Dictionary<T,int> term frequency
    ///   - HashSet<string>   distinct token set
    /// Tokenization is case-insensitive and alphanumeric.
    /// </summary>
    public sealed class SearchAnalyticsService
    {
        private static readonly SearchAnalyticsService _instance = new SearchAnalyticsService();
        public static SearchAnalyticsService Instance => _instance;

        private readonly Stack<string> _searchHistory = new Stack<string>();
        private readonly Dictionary<string, int> _termFrequency = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _distinctTerms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private SearchAnalyticsService() { }

        /// <summary>Pushes a search query onto the stack and updates frequencies.</summary>
        public void LogSearch(string query)
        {
            query = (query ?? string.Empty).Trim();
            _searchHistory.Push(query);

            foreach (var tok in Tokenize(query))
            {
                _distinctTerms.Add(tok);
                if (_termFrequency.ContainsKey(tok)) _termFrequency[tok]++;
                else _termFrequency[tok] = 1;
            }
        }

        /// <summary>Pops the last search (if any). Frequencies are kept as “interest memory”.</summary>
        public bool UndoLastSearch(out string removed)
        {
            if (_searchHistory.Count > 0)
            {
                removed = _searchHistory.Pop();
                return true;
            }
            removed = null;
            return false;
        }

        /// <summary>Returns top N terms by frequency (for debugging/evidence).</summary>
        public IEnumerable<KeyValuePair<string, int>> TopTerms(int n) =>
            _termFrequency.OrderByDescending(kv => kv.Value).Take(n);

        /// <summary>All distinct tokens seen so far.</summary>
        public IReadOnlyCollection<string> DistinctTerms => _distinctTerms;

        /// <summary>Lower-case tokenization of a query (alphanumeric only).</summary>
        public static IEnumerable<string> Tokenize(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) yield break;
            foreach (Match m in Regex.Matches(query.ToLowerInvariant(), @"[a-z0-9]+"))
                yield return m.Value;
        }
    }
}
