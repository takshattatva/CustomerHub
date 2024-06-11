namespace CustomerHub.DAL.ViewModels
{
    public class CustomerDTO
    {
        public List<CustomerList>? CustomerDashData { get; set; }
    }

    public class CustomerList
    {
        public int? AcId { get; set; }

        public string? AcCode { get; set; }

        public string? CompanyName { get; set; }

        public string? PostalCode { get; set; }

        public string? TelePhone { get; set; }

        public string? Relation { get; set; }

        public string? Currency { get; set; }

        public string? Country { get; set; }
    }
}