namespace TadrousManassa.Models
{
    public class Profile
    {
        public string Name { get; set; }
        public byte[]? Image { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public double TotalScore { get; set; }
        public int Rank { get; set; }
    }
}
