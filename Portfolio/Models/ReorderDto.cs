namespace Portfolio.Models
{
    public class ReorderDto
    {
        public int Id { get; set; }
        public string Direction { get; set; } = ""; // "up" or "down"
    }
}
