using System;
using System.IO;
using System.Linq;
using MunicipalServicesApp.Models;
using Newtonsoft.Json;

namespace MunicipalServicesApp.Services
{
    public static class ServiceRequestPersistence
    {
        private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MunicipalServicesApp", "servicerequests.json");

        public static void Save()
        {
            try
            {
                var repo = ServiceRequestRepository.Instance;
                var dir = Path.GetDirectoryName(FilePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(repo.AllRequests, Formatting.Indented));
            }
            catch
            {
                // swallow write errors to avoid crashing UI
            }
        }

        public static void Load()
        {
            try
            {
                if (!File.Exists(FilePath)) return;
                var json = File.ReadAllText(FilePath);
                var arr = JsonConvert.DeserializeObject<ServiceRequest[]>(json);
                if (arr == null) return;
                var repo = ServiceRequestRepository.Instance;
                foreach (var r in repo.AllRequests.ToList()) repo.RemoveRequest(r.Id);
                foreach (var r in arr)
                {
                    if (r.Id == Guid.Empty) r.Id = Guid.NewGuid();
                    if (r.CreatedAt == default) r.CreatedAt = DateTime.UtcNow;
                    repo.AddRequest(r);
                }
            }
            catch
            {
                // ignore load errors
            }
        }
    }
}
