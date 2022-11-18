using System.Collections.Generic;

namespace TestingExample.Controllers.Course.Dtos
{
    public record GetCoursesDto(
        IEnumerable<GetCourseDto> GetCourseDtos);
}