
namespace API.Helpers
{
    public class UserParams
    {
        // take it as query string from the client 
        // to clarify the paging properties
        private const int MaxPageSize = 50;
        private int _pageSize = 10;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }
        public int PageNumber { get; set; } = 1;
        public string CurrentUserName { get; set; }
        public string Gender { get; set; }
        public string OrderBy { get; set; } = "lastActive";

    }
}