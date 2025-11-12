using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Storage;

//For Data Persistence of Announcements using JSON serialization
//Kumar, J. (2024). Working with JSON in .NET Core: Newtonsoft.Json, NetJSON, and System.Text.Json. [online] C-sharpcorner.com. Available at: https://www.c-sharpcorner.com/article/net-core-tips-three-approaches-to-working-with-json-using-libraries-like-newto/ [Accessed 12 Nov. 2025].


namespace MunicipalServicesApp.Services
{
    public static class AnnouncementPersistence
    {
        private static readonly string FilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                         "MunicipalServicesApp", "announcements.json");

        public static void Save()
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var list = AnnouncementStore.Instance.GetAll().ToList();
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(list, Formatting.Indented));
        }

        public static void Load()
        {
            if (!File.Exists(FilePath)) return;

            try
            {
                var json = File.ReadAllText(FilePath);
                var list = JsonConvert.DeserializeObject<Announcement[]>(json);
                if (list == null) return;

                var store = AnnouncementStore.Instance;
                store.Announcements.Clear();

                foreach (var a in list)
                {
                    if (a.Id == Guid.Empty) a.Id = Guid.NewGuid();
                    if (a.PublishDate == default) a.PublishDate = DateTime.Today;
                    store.AddAnnouncement(a);
                }
            }
            catch
            {
                // ignore load errors
            }
        }
    }
}
