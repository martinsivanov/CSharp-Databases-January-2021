using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Officer")]
    public class ImportOfficerPrisonersDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        [XmlElement("Name")]
        public string Name { get; set; }
        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        [XmlElement("Money")]
        public decimal Money { get; set; }
        [Required]
        [XmlElement("Position")]
        public string Position { get; set; }
        [Required]
        [XmlElement("Weapon")]
        public string Weapon { get; set; }
        [Required]
        [XmlElement("DepartmentId")]
        public int DepartmentId { get; set; }
        [XmlArray("Prisoners")]
        public List<PrisonerDto> Prisoners { get; set; }

    }
    [XmlType("Prisoner")]
    public class PrisonerDto
    {
        [XmlAttribute("id")]
        public int PrisonerId { get; set; }
    }

  //    <Officer>
  //  <Name>Minerva Holl</Name>
  //  <Money>2582.55</Money>
  //  <Position>Overseer</Position>
  //  <Weapon>ChainRifle</Weapon>
  //  <DepartmentId>2</DepartmentId>
  //  <Prisoners>
  //    <Prisoner id = "15" />
  //  </ Prisoners >
  //</ Officer >

}
