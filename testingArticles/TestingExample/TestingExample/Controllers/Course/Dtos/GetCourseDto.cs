using System;

namespace TestingExample.Controllers.Course.Dtos
{
    public record GetCourseDto(
        Guid Id,
        string Title,
        int Credits,
        string DepartmentName);
}