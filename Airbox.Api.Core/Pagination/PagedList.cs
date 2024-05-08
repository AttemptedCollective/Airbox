namespace Airbox.Api.Core.Pagination
{
    public class PagedList<T> : List<T>
    {
        public int PageSize { get; private set; }

        public int PageNumber { get; private set; }

        public int TotalPages { get; private set; }

        public int TotalCount { get; private set; }

        public PagedList(IEnumerable<T> original, int pageSize, int pageNumber)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;

            TotalCount = original.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);

            AddRange(original.Skip((pageNumber - 1) * pageSize).Take(pageSize));
        }
    }
}
