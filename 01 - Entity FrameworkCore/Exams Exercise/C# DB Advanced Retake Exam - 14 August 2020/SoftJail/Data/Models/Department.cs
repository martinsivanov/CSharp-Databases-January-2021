using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoftJail.Data.Models
{
    public class Department
    {
        public Department()
        {
            this.Cells = new HashSet<Cell>();
        }

        [Key]
        public int Id { get; set; }

        [MaxLength(25)]
        [Required]
        public string Name { get; set; }

        public virtual ICollection<Cell> Cells { get; set; }
    }
//    •	Id – integer, Primary Key
//•	Name – text with min length 3 and max length 25 (required)
//•	Cells - collection of type Cell

}