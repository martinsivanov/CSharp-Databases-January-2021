using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeisterMask.DataProcessor.ImportDto
{
    public class ImportEmployeeDto
    {
        //•	Username - text with length [3, 40]. Should contain only lower or upper case letters and/or digits. (required)
        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        public string Username { get; set; }
        //•	Email – text (required). Validate it! There is attribute for this job.
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        //•	Phone - text. Consists only of three groups (separated by '-'), the first two consist of three digits and the last one - of 4 digits. (required)
        [Required]
        [RegularExpression(@"^[0-9]{3}\-[0-9]{3}\-[0-9]{4}$")]
        public string Phone { get; set; }
        public List<int> Tasks { get; set; }
    }
    //    "Username": "jstanett0",
    //"Email": "kknapper0@opera.com",
    //"Phone": "819-699-1096",
    //"Tasks": [
    //  34,
    //  32,
    //  65,
    //  30,
    //  30,
    //  45,
    //  36,

}
