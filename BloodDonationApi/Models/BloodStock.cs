namespace BloodDonationAPI.Models
{
    public class BloodStock
    {
        public int Id { get; set; }

        public string BloodType { get; set; } = string.Empty;

        public int Quantity { get; set; }
    }
}