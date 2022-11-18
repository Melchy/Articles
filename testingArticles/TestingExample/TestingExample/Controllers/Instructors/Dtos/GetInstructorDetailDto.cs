using System;
using System.Collections.Generic;

namespace TestingExample.Controllers.Instructors.Dtos
{
    public record GetInstructorDetailDto(
        Guid Id,
        string LastName,
        string FirsMidName,
        DateTime HireDate,
        string OfficeAssigmentLocation);
}
