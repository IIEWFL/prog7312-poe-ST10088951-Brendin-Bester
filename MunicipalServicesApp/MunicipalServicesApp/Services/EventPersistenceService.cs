using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Storage;

// For Data Persistence of EventItems using JSON serialization
//Kumar, J. (2024). Working with JSON in .NET Core: Newtonsoft.Json, NetJSON, and System.Text.Json. [online] C-sharpcorner.com. Available at: https://www.c-sharpcorner.com/article/net-core-tips-three-approaches-to-working-with-json-using-libraries-like-newto/ [Accessed 12 Nov. 2025].


namespace MunicipalServicesApp.Services
{
    public static class EventPersistenceService
    {
        private static readonly string FilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                         "MunicipalServicesApp", "events.json");

        public static void Save()
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var list = EventStore.Instance.GetAll().ToList();
            var json = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        public static void Load()
        {
            if (!File.Exists(FilePath)) return;

            try
            {
                var json = File.ReadAllText(FilePath);
                var list = JsonConvert.DeserializeObject<EventItem[]>(json);
                if (list == null) return;

                // Clear current in-memory store then add loaded items
                var store = EventStore.Instance;
                store.Events.Clear();
                store.Categories.Clear();

                foreach (var e in list)
                {
                    // Ensure Id and defaults
                    if (e.Id == Guid.Empty) e.Id = Guid.NewGuid();
                    if (e.Date == default) e.Date = DateTime.Today;
                    store.AddEvent(e);
                }
            }
            catch
            {
                // ignore load errors to avoid app crash; caller may log if desired
            }
        }
    }
}