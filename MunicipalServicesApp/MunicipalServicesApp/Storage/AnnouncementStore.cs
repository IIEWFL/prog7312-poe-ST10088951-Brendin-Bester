using MunicipalServicesApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

// In-memory store for Announcements
// Singleton pattern for global access
// dotnet-bot (2025). System.Collections.ObjectModel Namespace. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.collections.objectmodel?view=net-9.0 [Accessed 12 Nov. 2025].

namespace MunicipalServicesApp.Storage
{
    public sealed class AnnouncementStore
    {
        private static readonly Lazy<AnnouncementStore> lazy = new Lazy<AnnouncementStore>(() => new AnnouncementStore());
        public static AnnouncementStore Instance => lazy.Value;

        private AnnouncementStore() { }

        public ObservableCollection<Announcement> Announcements { get; } = new ObservableCollection<Announcement>();

        public IEnumerable<Announcement> GetAll() => Announcements.ToList();

        public void AddAnnouncement(Announcement item)
        {
            if (item == null) return;
            if (item.Id == Guid.Empty) item.Id = Guid.NewGuid();
            if (item.PublishDate == default) item.PublishDate = DateTime.Today;
            Announcements.Add(item);
        }

        public bool RemoveAnnouncementById(Guid id)
        {
            var existing = Announcements.FirstOrDefault(a => a.Id == id);
            if (existing == null) return false;
            Announcements.Remove(existing);
            return true;
        }

        public Announcement FindById(Guid id) => Announcements.FirstOrDefault(a => a.Id == id);
    }
}
