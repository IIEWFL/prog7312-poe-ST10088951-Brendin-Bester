using MunicipalServicesApp.DataStructures;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Storage
{
    /// <summary>
    /// Singleton in-memory store for all issue reports.
    /// </summary>
    public class DataStore
    {
        private static DataStore instance;

        private DataStore() { }

        /// <summary>
        /// Gets the single instance of the data store.
        /// </summary>
        public static DataStore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataStore();
                }
                return instance;
            }
        }

        /// <summary>
        /// All issues submitted during this session.
        /// </summary>
        public CustomLinkedList<ReportIssue> Issues { get; }
            = new CustomLinkedList<ReportIssue>();
    }
}
