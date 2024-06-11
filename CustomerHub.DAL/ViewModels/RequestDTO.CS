namespace CustomerHub.DAL.ViewModels
{
    public class RequestDTO<T> where T : class
    {
        public int PageNumber { get; set; } = 1;

        public int TotalPages { get; set; } = 1;

        public int TotalRecords { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public required string SearchValue { get; set; }

        public T SearchString { get; set; }

        public string OrderByAsc { get; set; } = "false";

        public int? SortColumn { get; set; }
    }
}
