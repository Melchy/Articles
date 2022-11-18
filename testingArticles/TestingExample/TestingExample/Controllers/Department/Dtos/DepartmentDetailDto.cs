using System;

namespace TestingExample.Controllers.Department.Dtos
{
    public record DepartmentDetailDto(
        string Name,
        decimal Budget,
        DateTime StartDate,
        Guid Id,
        string AdministratorFullName)
    {
    }
}