namespace VKPCShop.WebApp.Dtos
{
    public class PagingResult<T>
    {
        public List<T> Items { get; set; }
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int Count { get; set; }

        public int PageCount
        {
            get
            {
                var pageCount = (double)Count / PageSize;
                return (int)Math.Ceiling(pageCount);
            }
        }

    }
}
