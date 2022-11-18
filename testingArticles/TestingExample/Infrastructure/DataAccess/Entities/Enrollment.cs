using Infrastructure.DataAccess.Repositories;
using System;

namespace Infrastructure.DataAccess.Entities
{
    public class Enrollment  : Entity<Guid>
    {
        public Guid CourseId { get; set; }
        public Guid StudentId { get; set; }
        public Grade? Grade { get; set; }

        public Course Course { get; set; } = null!;
        public Student Student { get; set; } = null!;

        public Enrollment() : base(Guid.NewGuid())
        {
        }
    }
}
