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
        CreateMap<BookCreationDTO, Book>().
            ForMember(book => book.AuthorsBooks,
                options => options.MapFrom(AuthorBooksMap));
        
        CreateMap<Book, BookDTO>()
            .ForMember(bookDTO => bookDTO.Authors, options =>
                options.MapFrom(BookDTOAuthorsMap));
        CreateMap<CommentCreationDTO, Comment>();
        CreateMap<Comment, CommentDTO>();
    }

    private List<AuthorBook> AuthorBooksMap(BookCreationDTO bookCreationDto, Book book)
    {
        var result = new List<AuthorBook>();

        if (bookCreationDto.AuthorsIds == null) return result;

        foreach (var authorId in bookCreationDto.AuthorsIds)
        {
           result.Add(new AuthorBook() {AuthorId = authorId}); 
        }

        return result;
    }

    private List<AuthorDTO> BookDTOAuthorsMap(Book book, BookDTO bookDto)
    {
        var result = new List<AuthorDTO>();

        if (book.AuthorsBooks == null)   
            return result;

        foreach (var authorBook in book.AuthorsBooks)
        {
            result.Add(new AuthorDTO()
            {
                Id = authorBook.AuthorId,
                Name = authorBook.Author.Name
            });
        }

        return result;
    } 
}