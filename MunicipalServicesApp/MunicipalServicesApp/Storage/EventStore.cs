using MunicipalServicesApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MunicipalServicesApp.Storage
{
    public sealed class EventStore
    {
        private static readonly Lazy<EventStore> lazy = new Lazy<EventStore>(() => new EventStore());
        public static EventStore Instance => lazy.Value;

        private EventStore()
        {
            // seed collection kept empty here; persistence layer will load saved events
        }

        // Backing collection used by UI bindings
        public ObservableCollection<EventItem> Events { get; } = new ObservableCollection<EventItem>();

        // Expose categories (distinct)
        public ObservableCollection<string> Categories { get; } = new ObservableCollection<string>();

        // Optional stack to track last viewed
        public Stack<Guid> LastViewedStack { get; } = new Stack<Guid>();

        public IEnumerable<EventItem> GetAll() => Events.ToList();

        public void AddEvent(EventItem item)
        {
            if (item == null) return;

            // Ensure Id
            if (item.Id == Guid.Empty) item.Id = Guid.NewGuid();

            Events.Add(item);

            // maintain categories
            if (!string.IsNullOrWhiteSpace(item.Category) && !Categories.Contains(item.Category))
                Categories.Add(item.Category);
        }

        public bool RemoveEventById(Guid id)
        {
            var existing = Events.FirstOrDefault(e => e.Id == id);
            if (existing == null) return false;

            Events.Remove(existing);

            // Clean up categories if no events remain for that category
            var cat = existing.Category;
            if (!string.IsNullOrWhiteSpace(cat) && !Events.Any(e => string.Equals(e.Category, cat, StringComparison.OrdinalIgnoreCase)))
            {
                if (Categories.Contains(cat)) Categories.Remove(cat);
            }

            // Remove any occurrences from LastViewedStack
            var newStack = new Stack<Guid>(LastViewedStack.Where(g => g != id).Reverse());
            LastViewedStack.Clear();
            foreach (var g in newStack) LastViewedStack.Push(g);

            return true;
        }

        public EventItem FindById(Guid id) => Events.FirstOrDefault(e => e.Id == id);
    }
}
