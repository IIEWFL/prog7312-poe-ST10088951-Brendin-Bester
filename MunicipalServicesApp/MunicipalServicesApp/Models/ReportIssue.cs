using MunicipalServicesApp.DataStructures;
using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.Models
{
    /// <summary>
    /// Represents a single issue report submitted by the user.
    /// </summary>
    public class ReportIssue
    {
        public string Location { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime ReportedAt { get; set; }

        /// <summary>
        /// Stores file paths of attached media.
        /// </summary>
        public CustomLinkedList<string> MediaAttachments { get; }
            = new CustomLinkedList<string>();

        public IEnumerable<string> AttachmentPaths
           => MediaAttachments.Enumerate();

    }
}