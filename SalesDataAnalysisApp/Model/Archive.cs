﻿namespace SalesDataAnalysisApp.Models
{
    public class Archive
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int OwnerId { get; set; }
    }
}
