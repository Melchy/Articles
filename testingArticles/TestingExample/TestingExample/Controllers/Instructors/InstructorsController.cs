using Infrastructure.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ridge.Results;
using System;
using System.Linq;
using System.Threading.Tasks;
using TestingExample.Controllers.Course;
using TestingExample.Controllers.Department.Dtos;
using TestingExample.Controllers.Instructors.Dtos;

namespace TestingExample.Controllers.Department
{
    [ApiController]
    [Route("Department")]
    public class InstructorController : Controller
    {
        private readonly InstructorRepository _instructorRepository;
        private readonly EnrollmentsRepository _enrollmentsRepository;

        public InstructorController(
            InstructorRepository instructorRepository,
            EnrollmentsRepository enrollmentsRepository)
        {
            _instructorRepository = instructorRepository;
            _enrollmentsRepository = enrollmentsRepository;
        }

        [HttpGet("{id}/{courseId}")]
        public virtual async Task<GetInstructorsDto> GetInstructors(
            [FromRoute] Guid id,
            [FromQuery] Guid courseId)
        {
            var courses = await _instructorRepository.DbSet
                .Include(x => x.CourseAssignments)
                .ThenInclude(x => x.Course)
                .OrderBy(x => x.LastName)
                .ToListAsync();
            var enrollments = await _enrollmentsRepository.DbSet.Where(x => x.CourseId == courseId).ToListAsync();

            return new GetInstructorDto()
        }

        [HttpPost]
        public virtual async Task<ControllerResult<Guid>>
            UpdateOrInseserInstructor(
                [FromBody]
                UpdateOrInsertInstructorDto updateOrInsertInstructorDto)
        {

        }

        [HttpGet("{departmentId}")]
        public virtual async Task<ControllerResult<GetInstructorDetailDto>> InstructorDetail(
            [FromRoute]
            Guid instructorId)
        {
            var instructor = await _instructorRepository.GetInstructorWithOfficeAssigment(instructorId);
            return new GetInstructorDetailDto(instructor.Id,
                instructor.LastName,
                instructor.FirstMidName,
                instructor.HireDate,
                instructor.OfficeAssignment.Location);
        }
    }
}
