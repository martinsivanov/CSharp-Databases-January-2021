using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.Data.Models
{
    public class Game
    {
        //•	Id – integer, Primary Key
        public int Id { get; set; }
        //•	Name – text(required)
        [Required]
        public string Name { get; set; }

        //•	Price – decimal (non-negative, minimum value: 0) (required)
        public decimal Price { get; set; }

        //•	ReleaseDate – Date(required)
        [Required]
        public DateTime ReleaseDate { get; set; }
        //•	DeveloperId – integer, foreign key(required)
        public int DeveloperId { get; set; }
        //•	Developer – the game’s developer(required)
        [Required]
        public Developer Developer { get; set; }
        //•	GenreId – integer, foreign key(required)

        public int GenreId { get; set; }
        //•	Genre – the game’s genre(required)
        [Required]
        public Genre Genre { get; set; }
        //•	Purchases - collection of type Purchase

        public ICollection<Purchase> Purchases { get; set; }
        //•	GameTags - collection of type GameTag.Each game must have at least one tag.

        public ICollection<GameTag> GameTags { get; set; }


     

    }
}
