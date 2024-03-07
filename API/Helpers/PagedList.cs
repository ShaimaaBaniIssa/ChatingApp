

using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    public class PagedList<T> : List<T> // generic type
    {
        // return it to user when paging is needed 
        //  return list of items as needed
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            this.AddRange(items);
        }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source
        , int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();

            //get the items from the db by using ToList()
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            //For example:  if we are in the third page 
            //skip (3-1) * 4 --> will skip the first 8 items and then take the next 4

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }


    }
}