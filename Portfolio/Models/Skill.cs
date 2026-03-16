namespace Portfolio.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string title { get; set; }
        public int? EducationId { get; set; }
        public Education? Education { get; set; }
        public string? Category { get; set; }
        public int DisplayOrder { get; set; }
    }
}
