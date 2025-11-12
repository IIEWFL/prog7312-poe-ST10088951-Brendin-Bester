using System;

namespace MunicipalServicesApp.Models
{
    public enum RequestStatus
    {
        Submitted,
        Received,
        InProgress,
        OnHold,
        Completed,
        Rejected
    }

    public class ServiceRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Submitted;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int Priority { get; set; } = 5; // 1-highest .. 10-lowest

        public override string ToString() => $"{Title} ({Id}) - {Status}";
    }
}
