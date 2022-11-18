using System;

namespace TestingExample.Controllers.Department.Dtos
{
    public record EditDepartmentDto(string Name, decimal Budget, DateTime StartDate, Guid InstructorId);
}