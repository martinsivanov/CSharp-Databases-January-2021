namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ImportProjectDto>),
                  new XmlRootAttribute("Projects"));

            var projectsDto = (List<ImportProjectDto>)xmlSerializer.Deserialize(new StringReader(xmlString));
            var sb = new StringBuilder();
            var projects = new List<Project>();
            foreach (var projectDto in projectsDto)
            {
                if (!IsValid(projectDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime openDate;
                var isDateTimeValid = DateTime.TryParseExact(projectDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out openDate);
                if (!isDateTimeValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var project = new Project
                {
                    Name = projectDto.Name,
                    OpenDate = openDate
                };

                DateTime dueDate;
                DateTime? dueDateAsNull = null;
                if (!string.IsNullOrEmpty(projectDto.DueDate))
                {
                    var isDueDateValid = DateTime.TryParseExact(projectDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dueDate);
                    if (!isDueDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    dueDateAsNull = dueDate;
                    project.DueDate = dueDateAsNull;
                }
                else
                {
                    project.DueDate = dueDateAsNull;
                }
                var tasks = new List<Task>();
                foreach (var taskDto in projectDto.Tasks)
                {
                    if (!IsValid(taskDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var task = new Task
                    {
                        Name = taskDto.Name,
                        ExecutionType = (ExecutionType)taskDto.ExecutionType,
                        LabelType = (LabelType)taskDto.LabelType
                    };
                    DateTime taskOpenDate;
                    var istaskOpenDateValid = DateTime.TryParseExact(taskDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out taskOpenDate);
                    DateTime taskDueDate;
                    var istaskDueDateValid = DateTime.TryParseExact(taskDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out taskDueDate);
                    if (!istaskOpenDateValid || !istaskDueDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    //•	If there are any validation errors for the task entity(such as invalid name, open or due date are missing, 
                    //task open date is before project open date or task due date is after project due date), do not import it(only the task itself, not the whole project) and append an error message to the method output.
                    if (project.DueDate.HasValue)
                    {
                        if (taskDueDate > project.DueDate)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }
                    }
                    if (taskOpenDate < project.OpenDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    task.OpenDate = taskOpenDate;
                    task.DueDate = taskDueDate;

                    tasks.Add(task);
                }
                project.Tasks = tasks;
                projects.Add(project);
                sb.AppendLine(string.Format(SuccessfullyImportedProject, project.Name, project.Tasks.Count));
            }

            context.Projects.AddRange(projects);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var employeesDto = JsonConvert.DeserializeObject<ImportEmployeeDto[]>(jsonString);
            var sb = new StringBuilder();
            var employees = new List<Employee>();
            foreach (var employeeDto in employeesDto)
            {
                if (!IsValid(employeeDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                if (employeeDto.Username.All(ch => !char.IsLetterOrDigit(ch)))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var employee = new Employee
                {
                    Email = employeeDto.Email,
                    Phone = employeeDto.Phone,
                    Username = employeeDto.Username
                };
                foreach (var taskId in employeeDto.Tasks.Distinct())
                {
                    var task = context.Tasks.FirstOrDefault(x => x.Id == taskId);
                    if (task == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    employee.EmployeesTasks.Add(new EmployeeTask { Task = task, Employee = employee });
                }
                employees.Add(employee);
                sb.AppendLine(string.Format(SuccessfullyImportedEmployee, employee.Username, employee.EmployeesTasks.Count()));
            }
            context.Employees.AddRange(employees);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}