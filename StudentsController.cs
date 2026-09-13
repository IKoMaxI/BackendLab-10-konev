using Lab10.Api.Students.Contracts;
using Lab10.Api.Students.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab10.Api.Students.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController(ILogger<StudentsController> logger) : ControllerBase
    {
        private static readonly object SyncRoot = new();
        [HttpGet]
        public ActionResult<StudentResponseDTO[]> GetAllStudents()
        {
            lock (SyncRoot)
            {
                var students = _students.Select(x => new StudentResponseDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    Group = x.Group,
                    Specialization = x.Specialization
                }).ToArray();

                if (students.Any())
                {
                    logger.LogDebug("Количество студентов: {studentsCount}", students.Count());
                    logger.LogInformation("Студенты найдены и отображены в списке.");
                    return Ok(students);
                }

                logger.LogWarning("Невозможно получить список студентов: список пуст.");
                return NotFound("Список студентов пуст.");
            }
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<StudentResponseDTO> GetStudent(int id)
        {
            lock (SyncRoot)
            {
                var student = _students.FirstOrDefault(x => x.Id == id);

                if (student == null)
                {
                    logger.LogWarning("Студент с id {id} не найден.", id);
                    return NotFound("Студент не найден.");
                }

                logger.LogInformation(
                    "Студент найден | Id: {Id} | Имя: {Name}",
                    student.Id,
                    student.Name);

                return Ok(new StudentResponseDTO
                {
                    Id = student.Id,
                    Name = student.Name,
                    Group = student.Group,
                    Specialization = student.Specialization
                });
            }
        }

        [HttpPost]
        public ActionResult CreateStudent([FromBody] StudentRequestDTO contract)
        {
            lock (SyncRoot)
            {
                try
                {
                    var id = _students.Count == 0 ? 1 : _students.Max(x => x.Id) + 1;

                    var student = new Student
                    {
                        Id = id,
                        Name = contract.Name,
                        Group = contract.Group,
                        Specialization = contract.Specialization
                    };

                    logger.LogInformation(
                        "Создание студента | Id: {Id} | Имя: {Name} | Время: {Time}",
                        student.Id,
                        student.Name,
                        DateTime.UtcNow);

                    _students.Add(student);

                    return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, new StudentResponseDTO
                    {
                        Id = student.Id,
                        Name = student.Name,
                        Group = student.Group,
                        Specialization = student.Specialization
                    });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Ошибка при создании студента");
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }

        [HttpPut("{id}")]
        public ActionResult UpdateStudent(int id, [FromBody] StudentRequestDTO contract)
        {
            lock (SyncRoot)
            {
                var student = _students.FirstOrDefault(x => x.Id == id);

                if (student == null)
                {
                    logger.LogWarning("Студент с id {id} не найден.", id);
                    return NotFound();
                }

                student.Name = contract.Name;
                student.Group = contract.Group;
                student.Specialization = contract.Specialization;

                logger.LogInformation("Данные студента с id {id} обновлены.", student.Id);
                return NoContent();
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteStudent(int id)
        {
            lock (SyncRoot)
            {
                var index = _students.FindIndex(x => x.Id == id);

                if (index < 0)
                {
                    logger.LogWarning("Студент с id {id} не найден.", id);
                    return NotFound("Студент не найден.");
                }

                _students.RemoveAt(index);

                logger.LogInformation("Студент удалён | Id: {Id}", id);
                logger.LogDebug("Количество студентов: {studentsCount}", _students.Count());

                return NoContent();
            }
        }

        private static readonly List<Student> _students = [];
    }
}
