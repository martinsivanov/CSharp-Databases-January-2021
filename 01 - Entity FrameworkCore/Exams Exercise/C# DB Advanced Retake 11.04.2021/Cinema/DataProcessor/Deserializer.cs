namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Cinema.Data.Models;
    using Cinema.Data.Models.Enums;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportMovie
            = "Successfully imported {0} with genre {1} and rating {2}!";

        private const string SuccessfulImportProjection
            = "Successfully imported projection {0} on {1}!";

        private const string SuccessfulImportCustomerTicket
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var moviesDto = JsonConvert.DeserializeObject<ImportMovieDto[]>(jsonString);
            var sb = new StringBuilder();

            var movies = new List<Movie>();

            foreach (var movieDto in moviesDto)
            {
                if (!IsValid(movieDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var movieExist = movies.FirstOrDefault(x => x.Title == movieDto.Title);
                if (movieExist != null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                var movie = new Movie
                {
                    Title = movieDto.Title,
                    Genre = Enum.Parse<Genre>(movieDto.Genre),
                    Duration = TimeSpan.Parse(movieDto.Duration,CultureInfo.InvariantCulture),
                    Rating = movieDto.Rating,
                    Director = movieDto.Director
                };
                movies.Add(movie);
                sb.AppendLine(String.Format(SuccessfulImportMovie, movie.Title, movie.Genre, movie.Rating.ToString("f2")));
            }
            context.Movies.AddRange(movies);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ImportProjectionDto>),
                  new XmlRootAttribute("Projections"));

            var projectionsDto = (List<ImportProjectionDto>)xmlSerializer.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();
            var projections = new List<Projection>();

            foreach (var projectionDto in projectionsDto)
            {
                if (!IsValid(projectionDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var movie = context.Movies.FirstOrDefault(x => x.Id == projectionDto.MovieId);
                if (movie == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                DateTime dateTime;
               var isDateTimeValid =  DateTime.TryParseExact(projectionDto.DateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,DateTimeStyles.None, out dateTime);
                if (!isDateTimeValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var projection = new Projection
                {
                    Movie = movie,
                    MovieId = projectionDto.MovieId,
                    DateTime = dateTime
                };
                projections.Add(projection);
                sb.AppendLine(String.Format(SuccessfulImportProjection, projection.Movie.Title, projection.DateTime.ToString("MM/dd/yyyy",CultureInfo.InvariantCulture)));
            }
            context.Projections.AddRange(projections);
            context.SaveChanges();


            return sb.ToString().TrimEnd();
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ImportCustomerTicketsDto>),
                  new XmlRootAttribute("Customers"));

            var customersDto = (List<ImportCustomerTicketsDto>)xmlSerializer.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();
            var customers = new List<Customer>();

            foreach (var customerDto in customersDto)
            {
                if (!IsValid(customerDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                };
                var customer = new Customer
                {
                    FirstName = customerDto.FirstName,
                    LastName = customerDto.LastName,
                    Age = customerDto.Age,
                    Balance = customerDto.Balance
                };
                var tickets = new List<Ticket>();
                foreach (var ticketDto in customerDto.Tickets)
                {
                    if (!IsValid(ticketDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    var projection = context.Projections.FirstOrDefault(x => x.Id == ticketDto.ProjectionId);
                    if (projection == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    var ticket = new Ticket
                    {
                        Projection = projection,
                        ProjectionId = ticketDto.ProjectionId,
                        Price = ticketDto.Price
                    };
                    tickets.Add(ticket);
                }
                customer.Tickets = tickets;
                customers.Add(customer);
                sb.AppendLine(string.Format(SuccessfulImportCustomerTicket, customer.FirstName, customer.LastName, customer.Tickets.Count()));
            };

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }
        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}