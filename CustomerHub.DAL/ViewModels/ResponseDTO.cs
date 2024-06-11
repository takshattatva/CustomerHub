namespace CustomerHub.DAL.ViewModels
{
    public class ResponseDTO
    {
        public List<CustomerList>? TotalData { get; set; }

        public int TotalRecords { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public bool OrderByAsc { get; set; } = false;

        public string? SortColumn { get; set; }
    }
}
