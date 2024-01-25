namespace WebApiAuthor.DTOs
{
    public class HATEOASDate
    {
        public string Link { get; private set; }
        public string Description { get; private set; }
        public string Method { get; private set; }

        public HATEOASDate(string link, string description, string method)
        {
            Link = link;
            Description = description;
            Method = method;
        }
    }
}

