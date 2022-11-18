using System;
using System.Collections.Generic;

namespace TestingExample.Controllers.Instructors.Dtos
{
    public record UpdateOrInsertInstructorDto(
        string LastName,
        string FirstMidName,
        DateTime HireDate,
        string OfficeAssigmentLocation,
        IEnumerable<AssignedCourseData> SelectedCourses,
        IEnumerable<CourseAssignment> AssignedCourses
    );

    public record AssignedCourseData(
        Guid CourseId,
        string Title,
        bool Assigned);

    public record CourseAssignment(int CourseId);
}
