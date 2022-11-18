using Infrastructure.DataAccess.Entities;
using System;
using System.Collections.Generic;

namespace TestingExample.Controllers.Instructors.Dtos
{
    public record GetInstructorsDto(IEnumerable<GetInstructorDto> GetInstructorDtos);

    public record GetInstructorDto(
        Guid InstructorId,
        Guid CourseId,
        IEnumerable<InstructorDto> Instructors,
        IEnumerable<CourseDto> Course,
        IEnumerable<EnrollmentDto> Enrollments);

    public record InstructorDto(
        Guid Id,
        string LastName,
        string FirstMidName,
        DateTime HireDate,
        string OfficeAssigmentLocation,
        IEnumerable<CourseAssignmentDto> CourseAssignments);

    public record CourseAssignmentDto(
        Guid CourseId,
        string CourseTitle);

    public record CourseDto(
        Guid Id,
        string Title,
        string DepartmentName);

    public record EnrollmentDto(
        Grade Grade,
        string StudentFullName);

}
