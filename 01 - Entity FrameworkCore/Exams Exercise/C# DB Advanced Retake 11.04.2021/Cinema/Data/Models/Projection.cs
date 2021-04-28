using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.Data.Models
{
    public class Projection
    {

        public Projection()
        {
            this.Tickets = new HashSet<Ticket>();
        }
        public int Id { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
    //    •	Id – integer, Primary Key
    //•	MovieId – integer, Foreign key(required)
    //•	Movie – the Projection’s Movie
    //•	DateTime - DateTime(required)
    //•	Tickets - collection of type Ticket

}
