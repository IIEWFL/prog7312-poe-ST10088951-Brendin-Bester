using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Storage;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// Handles saving and loading of issue reports to a JSON file.
    /// </summary>
    public static class PersistenceService
    {
        private static readonly string DataFilePath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MunicipalServicesApp",
                "issues.json"
            );

        /// <summary>
        /// Serializes the in-memory list of issues to a JSON file.
        /// </summary>
        public static void SaveIssues()
        {
            var dir = Path.GetDirectoryName(DataFilePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var list = DataStore.Instance.Issues
                           .Enumerate()
                           .ToList();

            var json = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(DataFilePath, json);
        }

        /// <summary>
        /// Loads issues from the JSON file and repopulates the in-memory store.
        /// </summary>
        public static void LoadIssues()
        {
            if (!File.Exists(DataFilePath))
                return;

            var json = File.ReadAllText(DataFilePath);
            var list = JsonConvert.DeserializeObject<ReportIssue[]>(json);
            if (list == null)
                return;

            var store = DataStore.Instance;
            store.Issues.Clear();

            foreach (var issue in list)
                store.Issues.Add(issue);
        }
    }
}
