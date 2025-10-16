using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// Repository for Local Events & Announcements.
    /// </summary>
    public sealed class EventRepository
    {
        private static readonly EventRepository _instance = new EventRepository();
        public static EventRepository Instance => _instance;

        // Primary chronological store (keyed by date):
        private readonly SortedDictionary<DateTime, List<EventItem>> _byDate =
            new SortedDictionary<DateTime, List<EventItem>>();

        // Category -> events
        private readonly Dictionary<string, List<EventItem>> _byCategory =
            new Dictionary<string, List<EventItem>>(StringComparer.OrdinalIgnoreCase);

        // Set of all unique categories for the filter ComboBox:
        private readonly HashSet<string> _allCategories =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Recently added queue:
        private readonly Queue<EventItem> _recentQueue = new Queue<EventItem>();

        // Priority buckets: 0 (High), 1 (Medium), 2 (Low)
        private readonly SortedDictionary<int, Queue<EventItem>> _priorityBuckets =
            new SortedDictionary<int, Queue<EventItem>>();

        // Conveniences
        private readonly List<EventItem> _all = new List<EventItem>();

        private EventRepository()
        {
            // Ensure buckets exist even if empty:
            _priorityBuckets[0] = new Queue<EventItem>();
            _priorityBuckets[1] = new Queue<EventItem>();
            _priorityBuckets[2] = new Queue<EventItem>();
        }

        /// <summary>
        /// Seed a representative set of South African context events.
        /// </summary>
        public void Seed()
        {
            if (_all.Count > 0) return; // already seeded

            var today = DateTime.Today;

            var sample = new List<EventItem>
            {
                new EventItem {
                    Title = "Scheduled Water Outage - Bellville",
                    Category = "Water",
                    Description = "Maintenance on main line. Expect low pressure or outage.",
                    Date = today.AddDays(3),
                    Location = "Bellville CBD",
                    Priority = 0,
                    Tags = new HashSet<string>(new[]{"water","maintenance","outage","infrastructure"}, StringComparer.OrdinalIgnoreCase)
                },
                new EventItem {
                    Title = "Electricity Maintenance - Athlone Substation",
                    Category = "Electricity",
                    Description = "Substation upgrade; intermittent power cuts.",
                    Date = today.AddDays(5),
                    Location = "Athlone",
                    Priority = 1,
                    Tags = new HashSet<string>(new[]{"electricity","maintenance","upgrade","power"}, StringComparer.OrdinalIgnoreCase)
                },
                new EventItem {
                    Title = "Roadworks - N2 Ramp Resurfacing",
                    Category = "Roads",
                    Description = "Night-time resurfacing. Expect lane closures.",
                    Date = today.AddDays(7),
                    Location = "Somerset West N2 Ramp",
                    Priority = 1,
                    Tags = new HashSet<string>(new[]{"roads","resurfacing","closures","traffic"}, StringComparer.OrdinalIgnoreCase)
                },
                new EventItem {
                    Title = "Mobile Clinic Day",
                    Category = "Clinics",
                    Description = "Free screenings and vaccines. Bring ID/clinic card.",
                    Date = today.AddDays(10),
                    Location = "Khayelitsha Community Hall",
                    Priority = 2,
                    Tags = new HashSet<string>(new[]{"health","clinic","vaccines","community"}, StringComparer.OrdinalIgnoreCase)
                },
                new EventItem {
                    Title = "Library Reading Programme",
                    Category = "Libraries",
                    Description = "Youth reading circle and book exchange.",
                    Date = today.AddDays(12),
                    Location = "Mitchells Plain Library",
                    Priority = 2,
                    Tags = new HashSet<string>(new[]{"library","youth","reading","education"}, StringComparer.OrdinalIgnoreCase)
                },
                new EventItem {
                    Title = "Community Safety Awareness Evening",
                    Category = "Safety",
                    Description = "Neighbourhood watch orientation and safety tips.",
                    Date = today.AddDays(4),
                    Location = "Grassy Park Civic",
                    Priority = 1,
                    Tags = new HashSet<string>(new[]{"safety","community","awareness","neighbourhood"}, StringComparer.OrdinalIgnoreCase)
                },
                new EventItem {
                    Title = "Youth Worship Night",
                    Category = "Community",
                    Description = "Open-air youth gathering; music and testimony.",
                    Date = today.AddDays(9),
                    Location = "Macassar Sports Ground",
                    Priority = 1,
                    Tags = new HashSet<string>(new[]{"youth","community","event","worship"}, StringComparer.OrdinalIgnoreCase)
                },
                new EventItem {
                    Title = "Solid Waste Collection Delay",
                    Category = "Solid Waste",
                    Description = "Collection delayed by 1 day due to fleet maintenance.",
                    Date = today.AddDays(1),
                    Location = "Strandfontein",
                    Priority = 0,
                    Tags = new HashSet<string>(new[]{"waste","collection","delay","sanitation"}, StringComparer.OrdinalIgnoreCase)
                }
            };

            foreach (var e in sample) Add(e);
        }

        /// <summary>
        /// Adds an event into all data structures.
        /// </summary>
        public void Add(EventItem e)
        {
            if (e == null) return;

            // Flat list
            _all.Add(e);

            // Chronological index
            var dateKey = e.Date.Date;
            if (!_byDate.TryGetValue(dateKey, out var dateList))
            {
                dateList = new List<EventItem>();
                _byDate[dateKey] = dateList;
            }
            dateList.Add(e);

            // Category index
            var catKey = e.Category ?? string.Empty;
            if (!_byCategory.TryGetValue(catKey, out var catList))
            {
                catList = new List<EventItem>();
                _byCategory[catKey] = catList;
            }
            catList.Add(e);

            // Unique categories set
            if (!string.IsNullOrWhiteSpace(e.Category))
                _allCategories.Add(e.Category);

            // Recent FIFO
            _recentQueue.Enqueue(e);

            // Priority buckets
            if (!_priorityBuckets.TryGetValue(e.Priority, out var pq))
            {
                pq = new Queue<EventItem>();
                _priorityBuckets[e.Priority] = pq;
            }
            pq.Enqueue(e);
        }

        /// <summary>Returns all events (unsorted).</summary>
        public IEnumerable<EventItem> GetAll() => _all;

        /// <summary>Unique categories sorted by local culture for display.</summary>
        public IEnumerable<string> GetAllCategories()
        {
            var za = new CultureInfo("en-ZA");
            return _allCategories.OrderBy(s => s, StringComparer.Create(za, ignoreCase: true));
        }

        /// <summary>
        /// Inclusive date range over the chronological index.
        /// Iterates only the keys in range.
        /// </summary>
        public IEnumerable<EventItem> GetInDateRange(DateTime fromInclusive, DateTime toInclusive)
        {
            foreach (var kvp in _byDate)
            {
                if (kvp.Key < fromInclusive.Date) continue;
                if (kvp.Key > toInclusive.Date) break;
                foreach (var e in kvp.Value) yield return e;
            }
        }

        /// <summary>Quick lookup by category.</summary>
        public IEnumerable<EventItem> GetByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category)) return _all;
            return _byCategory.TryGetValue(category, out var list) ? list : Enumerable.Empty<EventItem>();
        }

        /// <summary>
        /// Combined search using text tokens.
        /// </summary>
        public IEnumerable<EventItem> Search(string query, string category, DateTime fromInclusive, DateTime toInclusive)
        {
            var tokens = SearchAnalyticsService.Tokenize(query).ToList();

            // Start with the date range to reduce set size early:
            var pool = GetInDateRange(fromInclusive, toInclusive);

            // Narrow by category:
            if (!string.IsNullOrWhiteSpace(category))
                pool = pool.Where(e => string.Equals(e.Category, category, StringComparison.OrdinalIgnoreCase));

            // If no tokens, return filtered list as-is:
            if (tokens.Count == 0)
                return pool.ToList();

            // Token match against title/description/tags:
            return pool.Where(e =>
            {
                var hay = $"{e.Title} {e.Description} {string.Join(" ", e.Tags)}".ToLowerInvariant();
                return tokens.Any(t => hay.Contains(t));
            }).ToList();
        }

        /// <summary>
        /// Returns items in priority order (High → Medium → Low), up to max.
        /// Dequeues from buckets.
        /// </summary>
        public IEnumerable<EventItem> DequeueUrgentOrder(int max)
        {
            var result = new List<EventItem>(max);
            foreach (var kvp in _priorityBuckets) // 0,1,2 in order
            {
                while (kvp.Value.Count > 0 && result.Count < max)
                    result.Add(kvp.Value.Dequeue());
                if (result.Count >= max) break;
            }
            return result;
        }

        /// <summary>Returns up to max recently added items (peek via LINQ Take).</summary>
        public IEnumerable<EventItem> GetRecent(int max) => _recentQueue.Take(Math.Max(0, max));
    }
}
