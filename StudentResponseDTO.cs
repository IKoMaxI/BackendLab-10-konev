namespace Lab10.Api.Students.Contracts
{
    public class StudentResponseDTO
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public required string Specialization { get; init; }
        public required int Group { get; init; }
    }
}
