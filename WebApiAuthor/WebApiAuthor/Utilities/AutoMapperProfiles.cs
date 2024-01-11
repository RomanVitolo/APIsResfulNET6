using AutoMapper;
using WebApiAuthor.DTOs;
using WebApiAuthor.Entities;

namespace WebApiAuthor.Utilities;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AuthorCreationDTO, Author>();
        CreateMap<Author, AuthorDTO>();
        CreateMap<BookCreationDTO, Book>();
        CreateMap<Book, BookDTO>();
    }         
}