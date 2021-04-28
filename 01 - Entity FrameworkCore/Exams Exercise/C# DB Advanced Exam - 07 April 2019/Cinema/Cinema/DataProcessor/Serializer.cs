namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Cinema.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var data = context
                .Movies
                .ToList()
                .Where(x => x.Rating >= rating && x.Projections.Any(p => p.Tickets.Count >= 1))
                .OrderByDescending(x => x.Rating)
                .ThenByDescending(x => x.Projections.Sum(p => p.Tickets.Sum(t => t.Price)))
                .Select(m => new
                {
                    MovieName = m.Title,
                    Rating = m.Rating.ToString("f2"),
                    TotalIncomes = m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)).ToString("f2"),
                    Customers = m.Projections
                    .SelectMany(c => c.Tickets)
                    .Select(x => new
                    {
                        FirstName = x.Customer.FirstName,
                        LastName = x.Customer.LastName,
                        Balance = x.Customer.Balance.ToString("f2")
                    })
                    .ToList()
                    .OrderByDescending(t => t.Balance)
                    .ThenBy(t => t.FirstName)
                    .ThenBy(t => t.LastName)
                    .ToList()
                })
                .Take(10)
                .ToList();

            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var data = context
                .Customers
                .ToList()
                .Where(x => x.Age >= age)
                .OrderByDescending(x => x.Tickets.Sum(c => c.Price))
                .Select(x => new ExportCustomerDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SpentMoney = x.Tickets.Sum(c => c.Price).ToString("f2"),
                    SpentTime = TimeSpan.FromMilliseconds(x.Tickets.Sum(t => t.Projection.Movie.Duration.TotalMilliseconds))
                        .ToString(@"hh\:mm\:ss")
                })
                .Take(10)
                .ToList();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ExportCustomerDto>), new XmlRootAttribute("Customers"));
            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            xmlSerializer.Serialize(new StringWriter(sb), data, namespaces);

            return sb.ToString().TrimEnd();

        }
    }
}