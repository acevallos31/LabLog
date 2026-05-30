namespace LabLog.Models
{
    public class LabNote
    {
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Observation { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public int Priority { get; set; }

        public bool IsPublic { get; set; }

        public string Tags { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public string UserId { get; set; } = string.Empty;
    }
}