using System;
using Infrastructure.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess
{
    public class ContosContext : DbContext
    {
        public DbSet<Course> Course { get; set; } = null!;
        public DbSet<CourseAssignment> CourseAssignments { get; set; } = null!;
        public DbSet<Student> Student { get; set; } = null!;
        public DbSet<Department> Department { get; set; } = null!;
        public DbSet<Enrollment> Enrollment { get; set; } = null!;
        public DbSet<Instructor> Instructor { get; set; } = null!;
        public DbSet<OfficeAssignment> OfficeAssignment { get; set; } = null!;

        public ContosContext(DbContextOptions<ContosContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Instructor>()
                .HasOne<OfficeAssignment>(x => x.OfficeAssignment)
                .WithOne(x => x.Instructor)
                .HasForeignKey<OfficeAssignment>(x => x.InstructorId);
        }
    }
}
