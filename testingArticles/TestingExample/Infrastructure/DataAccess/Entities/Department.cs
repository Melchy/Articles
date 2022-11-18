using Infrastructure.DataAccess.Repositories;
using System;
using System.Collections.Generic;

namespace Infrastructure.DataAccess.Entities
{
    public class Department : Entity<Guid>
    {
        public Instructor Instructor { get; set; }
        public string Name { get; set; } = null!;
        public decimal Budget { get; set; }
        public DateTime StartDate { get; set; }
        public Guid? InstructorId { get; set; }
        public Instructor Administrator { get; set; } = null!;
        public ICollection<Course> Courses { get; set; } = null!;

        public Department(
            string name,
            decimal budget,
            DateTime startDate,
            Instructor instructor) : base(Guid.NewGuid())
        {
            Name = name;
            Budget = budget;
            StartDate = startDate;
            Instructor = instructor;
        }
    }
}
