using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDto
{
    [XmlType("Customer")]
    public class ImportCustomerTicketsDto
    {
        //•	FirstName – text with length [3, 20] (required)
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [XmlElement("FirstName")]
        public string FirstName { get; set; }
        //•	LastName – text with length [3, 20] (required)
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [XmlElement("LastName")]
        public string LastName { get; set; }
        //•	Age – integer in the range [12, 110] (required)
        [Required]
        [Range(12, 110)]
        [XmlElement("Age")]
        public int Age { get; set; }
        //•	Balance - decimal (non-negative, minimum value: 0.01) (required)
        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        [XmlElement("Balance")]
        public decimal Balance { get; set; }
        [XmlArray("Tickets")]
        public ImportTicketDto[] Tickets { get; set; }
    }
    [XmlType("Ticket")]
    public class ImportTicketDto
    {
        [XmlElement("ProjectionId")]
        [Required]
        public int ProjectionId { get; set; }


        //•	Price – decimal (non-negative, minimum value: 0.01) (required)

        [Required]
        [Range(typeof(decimal),"0.01", "79228162514264337593543950335")]
        [XmlElement("Price")]
        public decimal Price { get; set; }
    }
    //  <Customer>
    //<FirstName>Randi</FirstName>
    //<LastName>Ferraraccio</LastName>
    //<Age>20</Age>
    //<Balance>59.44</Balance>
    //<Tickets>
    //  <Ticket>
    //    <ProjectionId>1</ProjectionId>
    //    <Price>7</Price>
    //  </Ticket>

}
