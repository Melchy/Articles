using System;

namespace TestingExample.Controllers.Department.Dtos
{
    public record CreateDepartmentDto(string Name, decimal Budget, DateTime StartDate, Guid InstructorId)
    {
    }
}
