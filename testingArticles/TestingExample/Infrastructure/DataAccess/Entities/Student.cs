using Infrastructure.DataAccess.Repositories;
using System;
using System.Collections.Generic;

namespace Infrastructure.DataAccess.Entities
{
    public class Student : Entity<Guid>
    {
        public string LastName { get; set; } = null!;
        public string FirstMidName { get; set; } = null!;
        public DateTime EnrollmentDate { get; set; }
        public string FullName => LastName + ", " + FirstMidName;

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        public Student() : base(Guid.NewGuid())
        {

        }
    }
}
