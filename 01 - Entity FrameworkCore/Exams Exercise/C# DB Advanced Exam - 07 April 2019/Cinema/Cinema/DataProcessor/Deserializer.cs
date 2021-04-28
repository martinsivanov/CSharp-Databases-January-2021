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
        private const string SuccessfulImportHallSeat
            = "Successfully imported {0}({1}) with {2} seats!";
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

                var movie = new Movie
                {
                    Title = movieDto.Title,
                    Genre = Enum.Parse<Genre>(movieDto.Genre),
                    Duration = TimeSpan.Parse(movieDto.Duration),
                    Rating = movieDto.Rating,
                    Director = movieDto.Director
                };
                if (movies.FirstOrDefault(x => x.Title == movie.Title) != null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                movies.Add(movie);
                sb.AppendLine(String.Format(SuccessfulImportMovie, movie.Title, movie.Genre, movie.Rating.ToString("f2")));
            }

            context.AddRange(movies);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var hallsSeatsDtos = JsonConvert.DeserializeObject<ImportHallSeatsDto[]>(jsonString);
            var sb = new StringBuilder();

            var halls = new List<Hall>();

            foreach (var hallSeatsDto in hallsSeatsDtos)
            {
                if (!IsValid(hallSeatsDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                if (hallSeatsDto.Seats <= 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var hall = new Hall
                {
                    Name = hallSeatsDto.Name,
                    Is3D = hallSeatsDto.Is3D,
                    Is4Dx = hallSeatsDto.Is4Dx,
                };
                for (int i = 0; i < hallSeatsDto.Seats; i++)
                {
                    hall.Seats.Add(new Seat());
                }

                var projectionType = string.Empty;
                if (hall.Is3D && hall.Is4Dx)
                {
                    projectionType = "4Dx/3D";
                }
                else if (hall.Is3D)
                {
                    projectionType = "3D";
                }
                else if (hall.Is4Dx)
                {
                    projectionType = "4Dx";
                }
                else
                {
                    projectionType = "Normal";
                }
                halls.Add(hall);
                sb.AppendLine(String.Format(SuccessfulImportHallSeat, hall.Name, projectionType, hall.Seats.Count));

            }
            context.Halls.AddRange(halls);
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
                if (!IsValid(projections))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var hall = context.Halls.FirstOrDefault(x => x.Id == projectionDto.HallId);
                var movie = context.Movies.FirstOrDefault(x => x.Id == projectionDto.MovieId);
                if (hall == null || movie == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var projection = new Projection
                {
                    Hall = hall,
                    HallId = projectionDto.HallId,
                    MovieId = projectionDto.MovieId,
                    Movie = movie,
                    DateTime = DateTime.ParseExact(projectionDto.DateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None)
                };

                projections.Add(projection);
                sb.AppendLine(String.Format(SuccessfulImportProjection, projection.Movie.Title, projection.DateTime.ToString("d", CultureInfo.InvariantCulture)));
            }
            context.Projections.AddRange(projections);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ImportCustomerTricketsDto>),
             new XmlRootAttribute("Customers"));

            var customersDto = (List<ImportCustomerTricketsDto>)xmlSerializer.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();

            var customers = new List<Customer>();

            foreach (var customerDto in customersDto)
            {
                if (!IsValid(customerDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

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
                    var ticket = new Ticket
                    {
                        Projection = projection,
                        ProjectionId = ticketDto.ProjectionId,
                        Price = ticketDto.Price,
                        Customer = customer
                    };
                    tickets.Add(ticket);
                }
                customer.Tickets = tickets;
                customers.Add(customer);
                sb.AppendLine(String.Format(SuccessfulImportCustomerTicket, customer.FirstName, customer.LastName, customer.Tickets.Count));
            }
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