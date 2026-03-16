namespace Portfolio.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string URL { get; set; }
        public string ImagePath { get; set; }
        public string? TechStack { get; set; }
        public int DisplayOrder { get; set; }
    }
}
