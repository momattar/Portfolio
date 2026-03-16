namespace Portfolio.Models
{
    public class Experience
    {
        public int Id { get; set; }
        public string Company { get; set; } = "";
        public string Role { get; set; } = "";
        public string DateRange { get; set; } = "";
        // Stored as newline-separated string, serialized to/from List<string>
        public string BulletsRaw { get; set; } = "";

        // Not mapped to DB — used for JSON serialization
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public List<string> Bullets
        {
            get => BulletsRaw.Split('\n', System.StringSplitOptions.RemoveEmptyEntries).ToList();
            set => BulletsRaw = string.Join('\n', value);
        }
    }
}
