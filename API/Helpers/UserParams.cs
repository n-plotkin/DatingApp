namespace API.Helpers
{

    //Class for the user to give us the parameters pagination info they want
    public class UserParams: PaginationParams
    {        
        public string CurentUsername { get; set; }
        public int MaxAge { get; set; } = 100;
        public int MinAge { get; set; } = 18;
        public string Gender { get; set; }
        public string OrderBy { get; set; }
    }
}