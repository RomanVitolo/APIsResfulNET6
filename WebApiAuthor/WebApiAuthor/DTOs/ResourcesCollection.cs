namespace WebApiAuthor.DTOs;

public class ResourcesCollection<T> : Resource where T : Resource
{
    public List<T> Values { get; set; }
}