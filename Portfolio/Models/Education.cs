namespace Portfolio.Models
{
    public class Education
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public List<Skill> Skills { get; set; }
        public int DisplayOrder { get; set; }
    }
}
