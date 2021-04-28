namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data";

        private const string SuccessfullyImportedDepartment = "Imported {0} with {1} cells";

        private const string SuccessfullyImportedPrisoner = "Imported {0} {1} years old";

        private const string SuccessfullyImportedOfficer = "Imported {0} ({1} prisoners)";
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var departmentsCellsDTO = JsonConvert
                .DeserializeObject<ImportDepartmentsCellsDTO[]>(jsonString);

            var departments = new List<Department>();

            foreach (var departmentCellsDTO in departmentsCellsDTO)
            {
                if (!IsValid(departmentCellsDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var department = new Department
                {
                    Name = departmentCellsDTO.Name
                };

                bool isDepInvalid = false;
                foreach (var cellDto in departmentCellsDTO.Cells)
                {
                    if (!IsValid(cellDto))
                    {
                        isDepInvalid = true;
                        sb.AppendLine(ErrorMessage);
                        break;
                    }

                    department.Cells.Add(new Cell
                    {
                        CellNumber = cellDto.CellNumber,
                        HasWindow = cellDto.HasWindow
                    });
                }
                if (isDepInvalid)
                {
                    continue;
                }
                if (department.Cells.Count == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                departments.Add(department);
                sb.AppendLine(String.Format(SuccessfullyImportedDepartment,
                    department.Name, department.Cells.Count));
            }

            context.Departments.AddRange(departments);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var PrisonersMailsDto = JsonConvert
                .DeserializeObject<List<ImportPrisonerMailsDto>>(jsonString);

            var prisoners = new List<Prisoner>();

            foreach (var prisonerDto in PrisonersMailsDto)
            {
                if (!IsValid(prisonerDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime IncarcerationDate;

                var isIncarcerationDateValid = DateTime.TryParseExact(prisonerDto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out IncarcerationDate);
                if (!isIncarcerationDateValid)
                {
                    continue;
                }

                DateTime? releaseDate = null;
                if (prisonerDto.ReleaseDate != null)
                {
                    DateTime releaseDateValue;
                    var isReleaseDateValid = DateTime.TryParseExact(prisonerDto.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out releaseDateValue);
                    if (!isReleaseDateValid)
                    {
                        continue;
                    }
                    releaseDate = releaseDateValue;
                }
                var prisoner = new Prisoner
                {
                    FullName = prisonerDto.FullName,
                    Nickname = prisonerDto.Nickname,
                    Age = prisonerDto.Age,
                    IncarcerationDate = IncarcerationDate,
                    ReleaseDate = releaseDate,
                    Bail = prisonerDto.Bail,
                    CellId = prisonerDto.CellId
                };
                bool isPrisonerValid = true;
                foreach (var mailDto in prisonerDto.MailDtos)
                {
                    if (!IsValid(mailDto))
                    {
                        isPrisonerValid = false;
                        sb.AppendLine(ErrorMessage);
                        break;
                    }
                    var mail = new Mail
                    {
                        Description = mailDto.Description,
                        Sender = mailDto.Sender,
                        Address = mailDto.Address
                    };
                    prisoner.Mails.Add(mail);
                }
                if (!isPrisonerValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                prisoners.Add(prisoner);
                sb.AppendLine(String.Format(SuccessfullyImportedPrisoner,
                    prisoner.FullName, prisoner.Age));

            }
            context.Prisoners.AddRange(prisoners);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ImportOfficerPrisonersDto>), new XmlRootAttribute("Officers"));


            var OfficersPrisonersDto = (List<ImportOfficerPrisonersDto>)xmlSerializer.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();

            var officers = new List<Officer >();

            foreach (var officerDto in OfficersPrisonersDto)
            {
                if (!IsValid(officerDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var isPositionValid = Enum.TryParse<Position>(officerDto.Position, out Position positionValue);
                if (!isPositionValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var isWeaponValid = Enum.TryParse<Weapon>(officerDto.Weapon, out Weapon weaponValue);
                if (!isWeaponValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var officer = new Officer
                {
                    FullName = officerDto.Name,
                    Salary = officerDto.Money,
                    Position = positionValue,
                    Weapon = weaponValue,
                    DepartmentId = officerDto.DepartmentId
                };

                foreach (var prisonerDto in officerDto.Prisoners)
                {
                    var prisoner = new OfficerPrisoner
                    {
                        Officer = officer,
                        PrisonerId = prisonerDto.PrisonerId
                    };
                    officer.OfficerPrisoners.Add(prisoner);
                }
                officers.Add(officer);
                sb.AppendLine(string.Format(SuccessfullyImportedOfficer, officer.FullName, officer.OfficerPrisoners.Count));
            }
            context.Officers.AddRange(officers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}