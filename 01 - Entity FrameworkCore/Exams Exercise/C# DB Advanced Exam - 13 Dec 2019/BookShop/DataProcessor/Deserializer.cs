namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportBookDto[]), new XmlRootAttribute("Books"));
            var booksDto = (ImportBookDto[])xmlSerializer.Deserialize(new StringReader(xmlString));
            var sb = new StringBuilder();
            var books = new List<Book>();
            foreach (var bookDto in booksDto)
            {
                if (!IsValid(bookDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var isValidDate = DateTime.TryParseExact(bookDto.PublishedOn,"MM/dd/yyyy",CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
                if (!isValidDate)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var book = new Book
                {
                    Name = bookDto.Name,
                    Genre = (Genre)bookDto.Genre,
                    Price = bookDto.Price,
                    Pages = bookDto.Pages,
                    PublishedOn = date
                };
                books.Add(book);
                sb.AppendLine(String.Format(SuccessfullyImportedBook, book.Name, book.Price));
            }
            context.Books.AddRange(books);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            var authorsDto = JsonConvert.DeserializeObject<ImportAuthorDto[]>(jsonString);

            var sb = new StringBuilder();

            var authors = new List<Author>();

            foreach (var authorDto in authorsDto)
            {
                if (!IsValid(authorDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                //•	If an email exists, do not import the author and append and error message.
                bool isEmailExist = GetEmail(authors, authorDto);
                if (isEmailExist)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                //•	If a book does not exist in the database, do not append an error message and continue with the next book

                var author = new Author
                {
                    FirstName = authorDto.FirstName,
                    LastName = authorDto.LastName,
                    Email = authorDto.Email,
                    Phone = authorDto.Phone
                };
                var books = new List<AuthorBook>();
                foreach (var bookDto in authorDto.Books)
                {
                    var book = context.Books.FirstOrDefault(x => x.Id == bookDto.BookId);
                    if (book == null)
                    {
                        continue;
                    }
                    var authorBook = new AuthorBook
                    {
                        Author = author,
                        Book = book
                    };
                    books.Add(authorBook);
                }
                author.AuthorsBooks = books;
                if (author.AuthorsBooks.Count == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                authors.Add(author);
                sb.AppendLine(String.Format(SuccessfullyImportedAuthor, 
                    author.FirstName + " " + author.LastName, author.AuthorsBooks.Count));

            }

            context.Authors.AddRange(authors);
            context.SaveChanges();

            return sb.ToString().TrimEnd();

        }

        private static bool GetEmail(List<Author> authors, ImportAuthorDto authorDto)
        {
            var email = authors.FirstOrDefault(x => x.Email == authorDto.Email);
            if (email == null)
            {
                return false;
            }
            return true;
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}