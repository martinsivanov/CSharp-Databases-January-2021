using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportPrisonerMailsDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string FullName { get; set; }

        [Required]
        [RegularExpression(@"^The\s[A-Z]{1}[a-z]*$")]
        public string Nickname { get; set; }
        [Range(18,65)]
        [Required]
        public int Age { get; set; }
        [Required]
        public string IncarcerationDate { get; set; }

        public string ReleaseDate { get; set; }
        [Range(typeof(decimal),"0","79228162514264337593543950335")]
        public decimal? Bail { get; set; }

        public int? CellId { get; set; }
        [JsonProperty("Mails")]
        public List<MailDto> MailDtos { get; set; }

    }
    public class MailDto
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public string Sender { get; set; }
        [Required]
        [RegularExpression(@"^((\w*\d*\s*)+(\s{1}str\.))$")]
        public string Address { get; set; }
    }

    //  {
    //"FullName": "Melanie Simonich",
    //"Nickname": "The Wallaby",
    //"Age": 32,
    //"IncarcerationDate": "29/03/1957",
    //"ReleaseDate": "27/03/2006",
    //"Bail": null,
    //"CellId": 5,
    //"Mails": [
    //  {
    //    "Description": "please add me to your LinkedIn network",
    //    "Sender": "Zonda Vasiljevic",
    //    "Address": "51677 Rieder Center str."
    //  },
    //  {
    //"Description": "Melanie i hope you found the best place for you!",
    //    "Sender": "Shell Lofthouse",
    //    "Address": "5877 Shoshone Way str."
    //  },
    //  {
    //"Description": "Turns out they wanted to implement things like fully responsive dynamic content, useful apps, etc – all things I told them they needed in the first place but which they opted not to include.",
    //    "Sender": "My Ansell",
    //    "Address": "71908 Waubesa Plaza str."
    //  }
    //]

}
