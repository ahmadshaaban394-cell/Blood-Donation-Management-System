namespace BloodDonationAPI.Models
{
    public class Donor
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string BloodType { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;
    }
}