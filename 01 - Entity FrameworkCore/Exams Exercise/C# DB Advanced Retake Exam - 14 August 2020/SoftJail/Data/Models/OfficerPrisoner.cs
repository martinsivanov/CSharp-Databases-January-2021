using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftJail.Data.Models
{
    public class OfficerPrisoner
    {
        [ForeignKey(nameof(Prisoner))]
        public int PrisonerId { get; set; }
        [Required]
        public virtual Prisoner Prisoner { get; set; }
        [ForeignKey(nameof(Officer))]
        public int OfficerId { get; set; }

        [Required]
        public virtual Officer Officer { get; set; }
    }

//    •	PrisonerId – integer, Primary Key
//•	Prisoner – the officer’s prisoner(required)
//•	OfficerId – integer, Primary Key
//•	Officer – the prisoner’s officer(required)

}