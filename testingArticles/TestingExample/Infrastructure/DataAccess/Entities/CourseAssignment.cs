using Infrastructure.DataAccess.Repositories;
using System;

namespace Infrastructure.DataAccess.Entities
{
    public class CourseAssignment : Entity<Guid>
    {
        public Guid InstructorId { get; set; }
        public Guid CourseId { get; set; }
        public Instructor Instructor { get; set; } = null!;
        public Course Course { get; set; } = null!;

        public CourseAssignment() : base(Guid.NewGuid())
        {
        }
    }
}
