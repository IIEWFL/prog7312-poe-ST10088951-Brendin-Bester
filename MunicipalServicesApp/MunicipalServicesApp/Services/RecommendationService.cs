using MunicipalServicesApp.Models;
using MunicipalServicesApp.Storage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

// Simple recommendation service based on user interactions
// Records category searches, date searches, and viewed events
// Recommends events based on weighted scoring of these interactions
//dotnet-bot (2025). System.Collections.Concurrent Namespace. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.collections.concurrent?view=net-9.0 [Accessed 12 Nov. 2025].


namespace MunicipalServicesApp.Services
{
    public sealed class RecommendationService
    {
        private static readonly Lazy<RecommendationService> lazy = new Lazy<RecommendationService>(() => new RecommendationService());
        public static RecommendationService Instance => lazy.Value;

        // Simple counters for categories and recent views
        private readonly ConcurrentDictionary<string, int> categoryScore = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private readonly LinkedList<Guid> recentViews = new LinkedList<Guid>();
        private readonly object lockObj = new object();

        private RecommendationService() { }

        public void RecordCategorySearch(string category)
        {
            if (string.IsNullOrWhiteSpace(category)) return;
            categoryScore.AddOrUpdate(category, 3, (k, v) => v + 3);
            TrimCategoryScores();
        }

        public void RecordDateSearch(DateTime date)
        {
            // small signal for date-based interest; map to a pseudo-category bucket
            var key = $"date:{date:yyyy-MM-dd}";
            categoryScore.AddOrUpdate(key, 1, (k, v) => v + 1);
            TrimCategoryScores();
        }

        public void RecordView(Guid eventId)
        {
            if (eventId == Guid.Empty) return;
            lock (lockObj)
            {
                recentViews.Remove(eventId);
                recentViews.AddFirst(eventId);
                while (recentViews.Count > 20) recentViews.RemoveLast();
            }
        }

        public IEnumerable<EventItem> RecommendTop(int count)
        {
            var store = EventStore.Instance;
            var candidates = store.GetAll().ToList();
            if (!candidates.Any()) return Enumerable.Empty<EventItem>();

            // Score each candidate by:
            // - category popularity (from categoryScore)
            // - recency (newer date)
            // - recent views boost
            var now = DateTime.UtcNow;
            List<(EventItem item, double score)> scored = new List<(EventItem, double)>();

            lock (lockObj)
            {
                foreach (var ev in candidates)
                {
                    double score = 0;

                    // category score
                    if (!string.IsNullOrWhiteSpace(ev.Category) && categoryScore.TryGetValue(ev.Category, out int cat))
                        score += cat * 1.5;

                    // date recency: more recent -> higher score (days difference)
                    var ageDays = (now - ev.Date.ToUniversalTime()).TotalDays;
                    score += Math.Max(0, 30 - ageDays) * 0.2;

                    // recent view boost
                    var pos = recentViews.ToList().FindIndex(g => g == ev.Id);
                    if (pos >= 0) score += (20 - pos) * 0.5;

                    // small tie-breaker from title length to make ordering deterministic
                    score += (ev.Title?.Length ?? 0) * 0.001;

                    scored.Add((ev, score));
                }
            }

            return scored
                .OrderByDescending(t => t.score)
                .ThenByDescending(t => t.item.Date)
                .Select(t => t.item)
                .Take(count);
        }

        private void TrimCategoryScores()
        {
            // keep only top 50 categories by score to avoid memory growth
            var top = categoryScore.OrderByDescending(kv => kv.Value).Take(50).Select(kv => kv.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var key in categoryScore.Keys)
            {
                if (!top.Contains(key))
                {
                    categoryScore.TryRemove(key, out _);
                }
            }
        }
    }
}