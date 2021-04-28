namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var data = context
                .Projects
                .ToArray()
                .Where(p => p.Tasks.Any())
                .Select(p => new ExportProjectDto
                {
                    TasksCount = p.Tasks.Count(),
                    ProjectName = p.Name,
                    HasEndDate = p.DueDate.HasValue ? "Yes" : "No",
                    Tasks = p.Tasks.Select(t => new ExportTaskDto 
                    {
                        Name = t.Name,
                        Label = t.LabelType.ToString()
                    })
                    .OrderBy(x => x.Name)
                    .ToArray()
                })
                .OrderByDescending(x => x.TasksCount)
                .ThenBy(x => x.ProjectName)
                .ToList();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ExportProjectDto>), new XmlRootAttribute("Projects"));
            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            xmlSerializer.Serialize(new StringWriter(sb), data, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var data = context
                .Employees
                .ToArray()
                .Where(x => x.EmployeesTasks.Any(t => t.Task.OpenDate >= date))
                .Select(e => new
                {
                    Username = e.Username,
                    Tasks = e.EmployeesTasks
                    .Where(x => (x.Task.OpenDate >= date))
                    .OrderByDescending(x => x.Task.DueDate)
                    .ThenBy(x => x.Task.Name)
                    .Select(x => new
                    {
                        TaskName = x.Task.Name,
                        OpenDate = x.Task.OpenDate.ToString("d",CultureInfo.InvariantCulture),
                        DueDate = x.Task.DueDate.ToString("d",CultureInfo.InvariantCulture),
                        LabelType = x.Task.LabelType.ToString(),
                        ExecutionType = x.Task.ExecutionType.ToString()
                    })
                    
                    .ToList()

                })
                .OrderByDescending(x => x.Tasks.Count)
                .ThenBy(x => x.Username)
                .Take(10)
                .ToList();



                //Select the top 10 employees who have at least one task that its open date is after or equal to the given date with their tasks that meet the same requirement(to have their open date after or equal to the giver date).For each employee, export their username and their tasks.For each task, export its name and open date(must be in format "d"), due date(must be in format "d"), label and execution type. Order the tasks by due date(descending), then by name(ascending).Order the employees by all tasks(meeting above condition) count(descending), then by username(ascending).


            return JsonConvert.SerializeObject(data,Formatting.Indented);
        }
    }
}