using System;

namespace SalesDataAnalysisApp.Models
{
    public class File
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Priority { get; set; }
        public string Category { get; set; }
        public byte[] Content { get; set; }
        public string OwnerName { get; set; }
        public string OwnerRole { get; set; }
        public int ArchiveId { get; set; }
    }
}
