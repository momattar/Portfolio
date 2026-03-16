namespace Portfolio.Models
{
    public class Certificate
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Issuer { get; set; } = "";
        public string? Url { get; set; }
        public int DisplayOrder { get; set; }
    }
}
