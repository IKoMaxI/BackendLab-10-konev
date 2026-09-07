using System.ComponentModel.DataAnnotations;

namespace Lab10.Api.Students.Contracts
{
    public class StudentRequestDTO
    {
        [Required]
        [MaxLength(40)]
        [MinLength(2)]
        [RegularExpression(@"^([A-ZА-Я][a-zа-я]*[ ]*)+$")]
        public required string Name { get; init; }

        [Required]
        [Range(1, 100)]
        public required int Group { get; init; }

        [Required]
        public required string Specialization { get; init; }
    }
}
