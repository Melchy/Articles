using System;

namespace TestingExample.Controllers.Course.Dtos
{
    public record CourseDetailDto(
        Guid Id,
        string Title,
        int Credits,
        string DepartmentName);
}
