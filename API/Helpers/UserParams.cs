namespace API.Helpers
{

    //Class for the user to give us the parameters pagination info they want
    public class UserParams
    {
        private const int MaxPageSize = 51;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;

        //let user set pagesize here, up to maxpagesize
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        
    }
}