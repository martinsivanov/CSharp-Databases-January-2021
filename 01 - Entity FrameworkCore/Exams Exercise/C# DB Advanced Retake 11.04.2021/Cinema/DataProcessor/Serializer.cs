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
                .ToArray()
                .Where(x => x.Rating >= rating && x.Projections.Any(x => x.Tickets.Any()))
                .OrderByDescending(x => x.Rating)
                .ThenByDescending(x => x.Projections.Sum(t => t.Tickets.Sum(x => x.Price)))
                .Select(m => new 
                {
                    MovieName = m.Title,
                    Rating = m.Rating.ToString("f2"),
                    TotalIncomes = m.Projections.Sum(t => t.Tickets.Sum(x => x.Price)).ToString("f2"),
                    Customers = m.Projections.SelectMany(x => x.Tickets)
                    .Select(c => new 
                    {
                        FirstName = c.Customer.FirstName,
                        LastName = c.Customer.LastName,
                        Balance = c.Customer.Balance.ToString("f2")
                    })
                    .OrderByDescending(x => x.Balance)
                    .ThenBy(x => x.FirstName)
                    .ThenBy(x => x.LastName)
                    .ToArray()
                })
                .Take(10);

            return JsonConvert.SerializeObject(data,Formatting.Indented);
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var data = context
                .Customers
                .ToArray()
                .Where(x => x.Age >= age)
                .OrderByDescending(x => x.Tickets.Sum(x => x.Price))
                .Select(c => new ExportCustomerDto
                {
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    SpentMoney = c.Tickets.Sum(x => x.Price).ToString("f2"),
                    SpentTime = TimeSpan.FromMilliseconds(c.Tickets.Sum(x => x.Projection.Movie.Duration.TotalMilliseconds)).ToString(@"hh\:mm\:ss")
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