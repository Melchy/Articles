using Infrastructure.DataAccess.Repositories;
using Iotc.SharedKernel.DomainExceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ridge.Results;
using System;
using System.Linq;
using System.Threading.Tasks;
using TestingExample.Controllers.Course.Dtos;

namespace TestingExample.Controllers.Course
{
    [ApiController]
    [Route("Course")]
    public class CourseController : Controller
    {
        private readonly CourseRepository _courseRepository;
        private readonly DepartmentRepository _departmentRepository;

        public CourseController(CourseRepository courseRepository,
            DepartmentRepository departmentRepository)
        {
            _courseRepository = courseRepository;
            _departmentRepository = departmentRepository;
        }

        [HttpGet]
        public virtual async Task<ControllerResult<GetCoursesDto>> GetCourses([FromQuery] int pageNumber)
        {
            if (pageNumber <= 0)
            {
                throw new DomainDataException(ErrorCode.InvalidPageNumber,
                    $"Page number must be higher than zero");
            }

            var pageSize = 10;
            var departments = await _courseRepository.DbSet
                .Include(x => x.Department)
                .OrderBy(x => x.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var departmentDtos = departments.Select(x => new GetCourseDto(x.Id,
                x.Title,
                x.Credits,
                x.Department.Name));
            return new GetCoursesDto(departmentDtos);
        }

        [HttpPost]
        public virtual async Task<ControllerResult<Guid>> CreateCourse(
            [FromBody] CreateCourseDto createCourseDto)
        {
            var department = await _departmentRepository.GetById(createCourseDto.Department);
            var course = new Infrastructure.DataAccess.Entities.Course(createCourseDto.Credits, createCourseDto.Title, department);
            await _courseRepository.Add(course);
            return course.Id;
        }

        [HttpDelete("{courseId}")]
        public virtual async Task<ControllerResult> DeleteCourse([FromRoute] Guid courseId)
        {
            var course = await _courseRepository.GetById(courseId);
            await _courseRepository.Remove(course);
            return Ok();
        }

        [HttpGet("{courseId}")]
        public virtual async Task<ControllerResult<CourseDetailDto>> GetCourseDetail(
            [FromRoute]
            Guid courseId)
        {
            var course = await _courseRepository.GetCourseWithDepartment(courseId);
            return new CourseDetailDto(course.Id, course.Title, course.Credits, course.Department.Name);
        }

        [HttpPut("{courseId}")]
        public virtual async Task<ControllerResult> EditCourse(
            [FromBody]
            EditCourseDto editCourseDto,
            [FromRoute]
            Guid courseId)
        {
            var course = await _courseRepository.GetById(courseId);
            course.Credits = editCourseDto.Credits ?? course.Credits;
            course.Title = editCourseDto.Title ?? course.Title;
            if (editCourseDto.DepartmentId != null)
            {
                var newDepartment = await _departmentRepository.GetById(editCourseDto.DepartmentId.Value);
                course.Department = newDepartment;
            }

            await _courseRepository.Update(course);
            return Ok();
        }
    }
}
