using Infrastructure.DataAccess.Entities;
using Iotc.SharedKernel.DomainExceptions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Infrastructure.DataAccess.Repositories
{
    public class DepartmentRepository : Repository<Department, Guid>
    {
        private readonly ContosContext _dbContext;

        public DepartmentRepository(
            ContosContext dbContext,
            IMediator mediator) : base(dbContext, mediator)
        {
            _dbContext = dbContext;
        }

        public async Task<Department> GetByIdWithAdministrator(
            Guid departmentId)
        {
            var department = await _dbContext.Department.Include(x => x.Administrator)
                .FirstOrDefaultAsync(x => x.Id == departmentId);
            if (department == null)
            {
                throw new DomainDataException(ErrorCode.EntityNotFound, $"Department with id {departmentId} not found");
            }

            return department;
        }
    }
}
