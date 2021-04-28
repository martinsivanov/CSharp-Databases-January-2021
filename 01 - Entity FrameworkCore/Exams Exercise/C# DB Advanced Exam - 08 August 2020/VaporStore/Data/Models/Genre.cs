﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.Data.Models
{
    public class Genre
    {

        //•	Id – integer, Primary Key
        public int Id { get; set; }
        //•	Name – text(required)
        [Required]
        public string Name { get; set; }
        //•	Games - collection of type Game
        public ICollection<Game> Games { get; set; }

    }
}
