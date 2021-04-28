namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            //var authors = context
            //    .Authors
            //    .ToList()
            //    .Select(x => new
            //    {
            //        AuthorName = x.FirstName + " " + x.LastName,
            //        Books = x.AuthorsBooks.Select(ab => new
            //        {
            //            BookName = ab.Book.Name,
            //            BookPrice = ab.Book.Price.ToString("f2")
            //        })
            //        .ToList()
            //    })
            //    .OrderByDescending(b => b.Books.Count)
            //    .ThenBy(a => a.AuthorName)
            //    .ToList();
            var authors = context
               .Authors
               .Select(a => new
               {
                   AuthorName = a.FirstName + ' ' + a.LastName,
                   Books = a.AuthorsBooks
                       .OrderByDescending(b => b.Book.Price)
                       .Select(b => new
                       {
                           BookName = b.Book.Name,
                           BookPrice = b.Book.Price.ToString("f2")
                       })
                       .ToList()

               })
               .ToList()
               .OrderByDescending(a => a.Books.Count())
               .ThenBy(a => a.AuthorName)
               .ToList();

            var json = JsonConvert.SerializeObject(authors, Formatting.Indented);

            return json;
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var books = context
                .Books
                .Where(b => b.PublishedOn < date && b.Genre == Genre.Science)
                .ToList()
                .OrderByDescending(x => x.Pages)
                .ThenByDescending(x => x.PublishedOn)
                .Select(x => new ExportBookDto
                {
                    Name = x.Name,
                    Date = x.PublishedOn.ToString("d", CultureInfo.InvariantCulture),
                    Pages = x.Pages
                })
                .Take(10)
                .ToList();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ExportBookDto>),
                new XmlRootAttribute("Books"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces();

            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new StringWriter(sb), books, namespaces);

            return sb.ToString().TrimEnd();

        }
    }
}