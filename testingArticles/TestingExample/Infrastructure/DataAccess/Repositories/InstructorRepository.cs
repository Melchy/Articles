using Infrastructure.DataAccess.Entities;
using Iotc.SharedKernel.DomainExceptions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Infrastructure.DataAccess.Repositories
{
    public class InstructorRepository : Repository<Instructor, Guid>
    {
        public InstructorRepository(
            ContosContext dbContext,
            IMediator mediator) : base(dbContext, mediator)
        {
        }

        public async Task<Instructor> GetInstructorWithOfficeAssigment(Guid id)
        {
            var instructor = await DbSet.Include(x => x.CourseAssignments)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (instructor == null)
            {
                throw new DomainErrorException(ErrorCode.EntityNotFound, $"Instuctor with id {id} not found");
            }

            return instructor;
        }
    }
}
