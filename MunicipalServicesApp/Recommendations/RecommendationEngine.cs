using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Recommendations
{
    public static class RecommendationEngine
    {
        public static IEnumerable<EventItem> Recommend(
            IEnumerable<EventItem> candidatePool,
            IEnumerable<string> currentSearchTokens,
            int maxResults = 5)
        {
            var tokens = (currentSearchTokens ?? Enumerable.Empty<string>())
                         .Distinct(StringComparer.OrdinalIgnoreCase)
                         .ToList();

            var today = DateTime.Today;

            double Score(EventItem e)
            {
                double score = 0;

                // 1) Term frequency match in title/description/tags
                if (tokens.Count > 0)
                {
                    var hay = $"{e.Title} {e.Description} {string.Join(" ", e.Tags)}".ToLowerInvariant();
                    foreach (var t in tokens)
                        if (hay.Contains(t)) score += 2.0;
                }

                // 2) Category overlap
                var tokenSet = new HashSet<string>(tokens, StringComparer.OrdinalIgnoreCase);
                var catSet = new HashSet<string>(e.Tags, StringComparer.OrdinalIgnoreCase);
                if (!string.IsNullOrWhiteSpace(e.Category)) catSet.Add(e.Category);

                double jaccard = 0;
                if (tokenSet.Count > 0 || catSet.Count > 0)
                {
                    var inter = tokenSet.Intersect(catSet, StringComparer.OrdinalIgnoreCase).Count();
                    var uni = tokenSet.Union(catSet, StringComparer.OrdinalIgnoreCase).Count();
                    jaccard = uni == 0 ? 0 : (double)inter / uni;
                }
                score += jaccard * 4.0;

                // 3) Recency
                var daysAhead = (e.Date.Date - today).TotalDays;
                if (daysAhead >= 0 && daysAhead <= 90)
                {
                    var rec = (90 - daysAhead) / 90.0; 
                    score += rec * 3.0;
                }

                // 4) Priority 
                score += (e.Priority == 0 ? 3 : (e.Priority == 1 ? 2 : 1));

                return score;
            }

            return candidatePool
                .OrderByDescending(Score)
                .ThenBy(e => e.Date) 
                .Take(Math.Max(0, maxResults))
                .ToList();
        }
    }
}
