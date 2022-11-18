using MediatR;
using System.Collections.Generic;

namespace Infrastructure.DataAccess.Repositories
{
    public interface IEventStore
    {
        public IEnumerable<INotification> Events { get; }
    }
}