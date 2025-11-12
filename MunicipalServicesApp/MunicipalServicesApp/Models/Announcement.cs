using System;

namespace MunicipalServicesApp.Models
{
    public class Announcement
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Body { get; set; }
        public string Category { get; set; }
        public DateTime PublishDate { get; set; } = DateTime.Today;
        public DateTime? ExpiryDate { get; set; }
        public string Author { get; set; }
    }
}
