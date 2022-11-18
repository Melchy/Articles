using Infrastructure.DataAccess.Repositories;
using System;

namespace Infrastructure.DataAccess.Entities
{
    public class OfficeAssignment : Entity<Guid>
    {
        public Guid InstructorId { get; set; }
        public string Location { get; set; } = null!;
        public Instructor Instructor { get; set; } = null!;

        public OfficeAssignment() : base(Guid.NewGuid())
        {
        }
    }
}
