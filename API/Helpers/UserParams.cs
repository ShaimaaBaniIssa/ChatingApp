
namespace API.Helpers
{
    public class UserParams : PaginationParams
    {
        // take it as query string from the client 
        // to clarify the paging properties

        public string CurrentUserName { get; set; }
        public string Gender { get; set; }
        public string OrderBy { get; set; } = "lastActive";

    }
}