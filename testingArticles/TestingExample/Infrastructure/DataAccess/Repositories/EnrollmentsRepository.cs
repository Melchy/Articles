using Infrastructure.DataAccess.Entities;
using MediatR;
using System;

namespace Infrastructure.DataAccess.Repositories
{
    public class EnrollmentsRepository : Repository<Enrollment, Guid>
    {
        public EnrollmentsRepository(
            ContosContext dbContext,
            IMediator mediator) : base(dbContext, mediator)
        {
        }
    }
}
