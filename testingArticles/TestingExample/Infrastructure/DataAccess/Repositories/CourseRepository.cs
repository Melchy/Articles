using Infrastructure.DataAccess.Entities;
using Iotc.SharedKernel.DomainExceptions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Infrastructure.DataAccess.Repositories
{
    public class CourseRepository : Repository<Course, Guid>
    {
        private readonly ContosContext _dbContext;

        public CourseRepository(
            ContosContext dbContext, IMediator mediator) : base(dbContext, mediator)
        {
            _dbContext = dbContext;
        }


        public async Task<Course> GetCourseWithDepartment(Guid courseId)
        {
            var course = await _dbContext.Course.Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.Id == courseId);
            if (course == null)
            {
                throw new DomainErrorException(ErrorCode.EntityNotFound,
                    $"Course not found. Id {courseId}");
            }

            return course;
        }
    }
}
