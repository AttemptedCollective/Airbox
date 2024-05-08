using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Airbox.Api.Core.Pagination
{
    /// <summary>
    /// Extension methods related to pagination within APIs.
    /// </summary>
    public static class PaginationExtensions
    {
        private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
        };

        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> to a <see cref="PagedList{T}"/>.
        /// </summary>
        /// <typeparam name="T">The item type for the paged response.</typeparam>
        /// <param name="list">The list to paginate.</param>
        /// <param name="pageSize">The number of entries on each page.</param>
        /// <param name="pageNumber">The page to retreive from the list. </param>
        /// <returns>The converted <see cref="PagedList{T}"/>.</returns>
        public static PagedList<T> ToPagedList<T>(this IEnumerable<T> list, int pageSize, int pageNumber) => new PagedList<T>(list, pageSize, pageNumber);

        /// <summary>
        /// Adds a serialised <see cref="PaginationHeader"/> to a <see cref="HttpResponse"/>. 
        /// </summary>
        /// <typeparam name="T">The item type for the paged response.</typeparam>
        /// <param name="response">The  <see cref="HttpResponse"/> to extend.</param>
        /// <param name="pagedList">The <see cref="PagedList{T}"/> used to create the <see cref="PaginationHeader"/>.</param>
        /// <returns>The extended <see cref="HttpResponse"/>.</returns>
        public static HttpResponse AddPaginationHeader<T>(this HttpResponse response, PagedList<T>? pagedList)
        {
            if (pagedList is not null)
            {
                var paginationHeader = new PaginationHeader(pagedList.PageNumber, pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages);

                response.Headers.Append("pagination", JsonConvert.SerializeObject(paginationHeader, _jsonSerializerSettings));
                // FUTURE: Could add Access-Control-Expose-Headers for CORS
            }

            return response;
        }
    }
}
