namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners = context
                .Prisoners
                .ToList()
                .Where(p => ids.Contains(p.Id))
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers.Select(o => new 
                    {
                        OfficerName = o.Officer.FullName,
                        Department = o.Officer.Department.Name
                    })
                    .OrderBy(x => x.OfficerName)
                    .ToList(),
                    TotalOfficerSalary = p.PrisonerOfficers.Sum(x => x.Officer.Salary)
                    
                })
                .OrderBy(x => x.Name)
                .ThenBy(p => p.Id)
                .ToList();

            var jsonString = JsonConvert.SerializeObject(prisoners,Formatting.Indented);

            return jsonString;
                
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var prisonersNameArr = prisonersNames.Split(",").ToArray();

            var prisoners = context
                .Prisoners
                .Where(p => prisonersNameArr.Contains(p.FullName))
                .ToArray()
                .Select(p => new PrisonerDto
                {
                    Id = p.Id,
                    Name = p.FullName,
                    IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd",CultureInfo.InvariantCulture),
                    Mails = p.Mails.Select(m => new MailDto
                    {
                        Description = ReverseString(m.Description)
                    }).ToArray()
                })
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PrisonerDto[]), new XmlRootAttribute("Prisoners"));

            var sb = new StringBuilder();

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);
            xmlSerializer.Serialize(new StringWriter(sb), prisoners,namespaces);

            return sb.ToString().TrimEnd();
        }

        private static string ReverseString(string s)
        {
            var charArray = s.ToCharArray();

            Array.Reverse(charArray);

            return new string(charArray);
        }
    }
}