using JetBrains.Annotations;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DataAccess.Repositories
{
    public abstract class Entity<TKey> : IEventStore
    {
        protected Entity(
            TKey id)
        {
            Id = id;
            Created = Clock.UtcNow;
        }

        public TKey Id { get; protected set; }
        protected DateTimeOffset Created { get; set; }

        [UsedImplicitly]
        public bool IsDeleted { get; protected set; }
        [NotMapped]
        protected List<INotification> Events { get; } = new List<INotification>();
        [NotMapped]
        IEnumerable<INotification> IEventStore.Events => Events;
    }
}