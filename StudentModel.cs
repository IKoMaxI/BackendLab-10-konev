namespace Lab10.Api.Students.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Group { get; set; }
        public string Specialization { get; set; } = string.Empty;
    }
}
