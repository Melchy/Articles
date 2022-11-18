using Infrastructure.DataAccess.Repositories;
using Iotc.SharedKernel.DomainExceptions;
using System;
using System.Collections.Generic;

namespace Infrastructure.DataAccess.Entities
{
    public class Course : Entity<Guid>
    {
        public string Title { get; set; } = null!;
        public int Credits { get; set; }
        public Guid DepartmentId { get; private set; }
        public Department Department { get; set; } = null!;
        public ICollection<Enrollment> Enrollments { get; private set; } = null!;
        public ICollection<CourseAssignment> CourseAssignments { get; private set; } = null!;

        private Course() : base(Guid.NewGuid())
        {
        }

        public Course(int credits, string title, Department department) : base(Guid.NewGuid())
        {
            Credits = credits;
            Title = title;
            Department = department;
        }
    }
}
