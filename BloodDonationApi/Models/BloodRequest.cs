namespace BloodDonationAPI.Models
{
    public class BloodRequest
    {
        public int Id { get; set; }

        public string PatientName { get; set; } = string.Empty;

        public string BloodType { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public string HospitalName { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";
    }
}