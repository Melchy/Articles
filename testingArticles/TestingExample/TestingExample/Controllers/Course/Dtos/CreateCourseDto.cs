using System;

namespace TestingExample.Controllers.Course.Dtos
{
    public record CreateCourseDto(
        int Credits,
        Guid Department,
        string Title);
}