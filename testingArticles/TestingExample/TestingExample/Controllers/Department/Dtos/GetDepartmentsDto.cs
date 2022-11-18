using System.Collections.Generic;

namespace TestingExample.Controllers.Department.Dtos
{
    public record GetDepartmentsDto(IEnumerable<GetDepartmentDto> GetDepartmentDtos);
}