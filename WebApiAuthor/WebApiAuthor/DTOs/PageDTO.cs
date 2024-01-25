namespace WebApiAuthor.DTOs
{
    public class PageDTO
    {
        public int Page { get; set; } = 1;
        private int _recordsPerPage = 10;
        private readonly int _maxAmountPerPage = 50;

        public int RecordsPerPage
        {
            get => _recordsPerPage;
            set => _recordsPerPage = (value > _maxAmountPerPage) ? _maxAmountPerPage : value;
        }
    }
}

