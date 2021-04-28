namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
    {
        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var gamesDto = JsonConvert.DeserializeObject<ImportGameDto[]>(jsonString);

            var sb = new StringBuilder();

            foreach (var gameDto in gamesDto)
            {
                if (!IsValid(gameDto) || gameDto.Tags.Count == 0)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var releaseDate = DateTime.ParseExact(gameDto.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //•	If a developer/genre/tag with that name doesn’t exist, create it. 
                var developer = GetDeveloper(context, gameDto);
                var genre = GetGenre(context, gameDto);

                var game = new Game
                {
                    Name = gameDto.Name,
                    Price = gameDto.Price,
                    ReleaseDate = releaseDate,
                    Developer = developer,
                    Genre = genre
                };
                var gameTags = new List<GameTag>();
                foreach (var currentTag in gameDto.Tags)
                {
                    var tag = GetTag(context, currentTag);
                    var gameTag = new GameTag
                    {
                        Game = game,
                        Tag = tag
                    };
                    gameTags.Add(gameTag);
                }
                game.GameTags = gameTags;
                context.Games.AddRange(game);
                context.SaveChanges();
                sb.AppendLine($"Added {game.Name} ({game.Genre.Name}) with {game.GameTags.Count} tags");
            }


            return sb.ToString().TrimEnd();
        }


        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var UsersDto = JsonConvert.DeserializeObject<ImportUserDto[]>(jsonString);

            var sb = new StringBuilder();
            var users = new List<User>();
            foreach (var userDto in UsersDto)
            {
                if (!IsValid(userDto)
                    || userDto.Cards.Count() == 0)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                var user = new User
                {
                    FullName = userDto.FullName,
                    Username = userDto.Username,
                    Email = userDto.Email,
                    Age = userDto.Age
                };
                var isUserValid = false;

                var cards = new List<Card>();
                foreach (var currentCard in userDto.Cards)
                {
                    if (!IsValid(currentCard))
                    {
                        isUserValid = true;
                        break;
                    }
                    var isValidType = Enum.TryParse<CardType>(currentCard.Type,
                        out CardType cardType);

                    if (!isValidType)
                    {
                        isUserValid = true;
                        break;
                    }
                    var card = new Card
                    {
                        Number = currentCard.Number,
                        Cvc = currentCard.CVC,
                        Type = cardType
                    };

                    cards.Add(card);
                }
                if (isUserValid)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                user.Cards = cards;
                users.Add(user);
                sb.AppendLine($"Imported {user.Username} with {user.Cards.Count} cards");
            }

            context.Users.AddRange(users);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPurchaseDto[]), new XmlRootAttribute("Purchases"));
            StringReader stringReader = new StringReader(xmlString);
            var sb = new StringBuilder();

            var purchasesDto = (ImportPurchaseDto[])xmlSerializer.Deserialize(stringReader);
            var purchases = new List<Purchase>();
            foreach (var purchaseDto in purchasesDto)
            {
                if (!IsValid(purchaseDto))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                var date = DateTime.ParseExact(purchaseDto.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                var type = Enum.Parse<PurchaseType>(purchaseDto.Type);

                var game = getGame(context, purchaseDto);

                var purchase = new Purchase
                {
                    Game = game,
                    Type = type,
                    ProductKey = purchaseDto.Key,
                    Card = context.Cards.First(x => x.Number == purchaseDto.Card),
                    Date = date
                };
                purchases.Add(purchase);
                sb.AppendLine($"Imported {purchase.Game.Name} for {purchase.Card.User.Username}");
            }
            context.Purchases.AddRange(purchases);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static Game getGame(VaporStoreDbContext context, ImportPurchaseDto purchaseDto)
        {
           return context.Games.FirstOrDefault(x => x.Name == purchaseDto.Title);
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
        private static Tag GetTag(VaporStoreDbContext context, string currentTag)
        {
            var tag = context.Tags.FirstOrDefault(x => x.Name == currentTag);
            if (tag == null)
            {
                tag = new Tag
                {
                    Name = currentTag
                };
                context.Tags.Add(tag);
                context.SaveChanges();
            }
            return tag;
        }

        private static Genre GetGenre(VaporStoreDbContext context, ImportGameDto gameDto)
        {
            var genre = context.Genres.FirstOrDefault(x => x.Name == gameDto.Genre);
            if (genre == null)
            {
                genre = new Genre
                {
                    Name = gameDto.Genre
                };
                context.Genres.Add(genre);
                context.SaveChanges();
            }
            return genre;
        }

        private static Developer GetDeveloper(VaporStoreDbContext context, ImportGameDto gameDto)
        {
            var developer = context.Developers.FirstOrDefault(x => x.Name == gameDto.Developer);
            if (developer == null)
            {
                developer = new Developer
                {
                    Name = gameDto.Developer
                };
                context.Developers.Add(developer);
                context.SaveChanges();
            }
            return developer;
        }
    }
}