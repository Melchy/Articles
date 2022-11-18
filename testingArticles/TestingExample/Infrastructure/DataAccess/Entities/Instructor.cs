using Infrastructure.DataAccess.Repositories;
using System;
using System.Collections.Generic;

namespace Infrastructure.DataAccess.Entities
{
    public class Instructor : Entity<Guid>
    {
        public string LastName { get; set; } = null!;
        public string FirstMidName { get; set; } = null!;
        public DateTime HireDate { get; set; }
        public string FullName => LastName + ", " + FirstMidName;
        public ICollection<CourseAssignment> CourseAssignments { get; private set; } = new List<CourseAssignment>();
        public OfficeAssignment OfficeAssignment { get; private set; } = null!;

        public Instructor() : base(Guid.NewGuid())
        {
        }
    }
}
