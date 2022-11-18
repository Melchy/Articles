using Infrastructure.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ridge.Results;
using System;
using System.Linq;
using System.Threading.Tasks;
using TestingExample.Controllers.Course;
using TestingExample.Controllers.Department.Dtos;
namespace TestingExample.Controllers.Department
{
    [ApiController]
    [Route("Department")]
    public class DepartmentController : Controller
    {
        private readonly DepartmentRepository _departmentRepository;
        private readonly InstructorRepository _instructorRepository;

        public DepartmentController(
            DepartmentRepository departmentRepository,
            InstructorRepository instructorRepository)
        {
            _departmentRepository = departmentRepository;
            _instructorRepository = instructorRepository;
        }

        [HttpGet]
        public virtual async Task<GetDepartmentsDto> GetDepartments(
            [FromQuery]
            int pageNumber)
        {
            var pageSize = 10;
            var departments = await _departmentRepository.DbSet
                .Include(x => x.Administrator)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageNumber)
                .ToListAsync();
            var getDepartmentDtos = departments.Select(x =>
                new GetDepartmentDto(x.Name,
                    x.Budget,
                    x.StartDate,
                    x.Id,
                    x.Administrator.FullName));
            return new GetDepartmentsDto(getDepartmentDtos);
        }

        [HttpPost]
        public virtual async Task<ControllerResult<Guid>> CreateDepartment([FromBody] CreateDepartmentDto createDepartmentDto)
        {
            var instructor = await _instructorRepository.GetById(createDepartmentDto.InstructorId);
            var department = new Infrastructure.DataAccess.Entities.Department(
                createDepartmentDto.Name,
                createDepartmentDto.Budget,
                createDepartmentDto.StartDate,
                instructor);
            await _departmentRepository.Add(department);
            return department.Id;
        }

        [HttpDelete("{departmentId}")]
        public virtual async Task<ControllerResult> DeleteDepartment([FromRoute] Guid departmentId)
        {
            var department = await _departmentRepository.GetById(departmentId);
            await _departmentRepository.Remove(department);
            return Ok();
        }

        [HttpGet("{departmentId}")]
        public virtual async Task<ControllerResult<DepartmentDetailDto>> DepartmentDetail(
            [FromRoute]
            Guid departmentId)
        {
            var department = await _departmentRepository.GetByIdWithAdministrator(departmentId);
            return new DepartmentDetailDto(department.Name,
                department.Budget,
                department.StartDate,
                department.Id,
                department.Administrator.FullName);
        }

        [HttpPut("{depratmentId}")]
        public virtual async Task<ControllerResult> EditCourse(
            [FromBody]
            EditDepartmentDto editDepartmentDto,
            [FromRoute]
            Guid departmentId)
        {
            var department = await _departmentRepository.GetById(departmentId);
            var instructor = await _instructorRepository.GetById(editDepartmentDto.InstructorId);
            department.Budget = editDepartmentDto.Budget;
            department.Name = editDepartmentDto.Name;
            department.StartDate = editDepartmentDto.StartDate;
            department.Instructor = instructor;
            return Ok();
        }
    }
}
