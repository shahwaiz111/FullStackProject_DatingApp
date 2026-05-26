namespace MyDatingApp.Api.Helpers
{
    public class PagingParams
    {
        private const int MaxPageSize = 50; // Max page size can be 50
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10; // 10 items per page..

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

    }
}
