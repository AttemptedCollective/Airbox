namespace Airbox.Api.Core.Pagination
{
    public class PageParameters
    {
        const int _minPageNumber = 1;
        const int _minPageSize = 1;
        const int _maxPageSize = 50;

        private int _pageNumber = 1;
        private int _pageSize = 1;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value > _minPageNumber ? value : _minPageNumber;
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {

                if (value > _maxPageSize)
                {
                    _pageSize = _maxPageSize;
                }
                else if (value < _minPageSize)
                {
                    _pageSize = _minPageSize;
                }
                else
                {
                    _pageSize = value;
                }
            }
        }
    }
}
