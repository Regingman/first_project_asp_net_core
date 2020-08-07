using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>()
                        .HasOne(m => m.Department)
                        .WithMany(t => t.Groups)
                        .HasForeignKey(m => m.Id_department);

            modelBuilder.Entity<Department>()
                        .HasOne(m => m.Faculty)
                        .WithMany(t => t.Departments)
                        .HasForeignKey(m => m.Id_faculty)
                        ;

            modelBuilder.Entity<ListLesson>()
                        .HasOne(m => m.Groups)
                        .WithMany(t => t.ListLessons)
                        .HasForeignKey(m => m.Id_group)
                        ;

            modelBuilder.Entity<ListLesson>()
                        .HasOne(m => m.Lessons)
                        .WithMany(t => t.ListLessons)
                        .HasForeignKey(m => m.Id_lesson)
                        ;

            modelBuilder.Entity<ListLesson>()
                        .HasOne(m => m.Teachers)
                        .WithMany(t => t.ListLessons)
                        .HasForeignKey(m => m.Id_teacher)
                        ;
            base.OnModelCreating(modelBuilder);

        }
        public virtual DbSet<Group> Groups { get; set; }

        public virtual DbSet<Faculty> Faculties { get; set; }

        public virtual DbSet<Department> Departments { get; set; }

        public virtual DbSet<Lesson> Lessons { get; set; }

        public virtual DbSet<ListLesson> ListLessons { get; set; }

        public virtual DbSet<Teacher> Teachers { get; set; }
    }


    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int Id_department { get; set; }

        public virtual Department Department { get; set; }

        public virtual ICollection<ListLesson> ListLessons { get; set; }

    }

    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ListLesson> ListLessons { get; set; }
    }

    public class Lesson
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ListLesson> ListLessons { get; set; }

    }

    public class ListLesson
    {
        public int Id { get; set; }
        public int NumberLesson { get; set; }
        public int DayOfWeek { get; set; }
        public int Id_group { get; set; }

        public int Id_teacher { get; set; }

        public int Id_lesson { get; set; }

        public virtual Group Groups { get; set; }

        public virtual Lesson Lessons { get; set; }

        public virtual Teacher Teachers { get; set; }
    }

    public class Faculty
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Department> Departments { get; set; }
    }
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public virtual ICollection<Group> Groups { get; set; }

        public int Id_faculty { get; set; }
        public virtual Faculty Faculty { get; set; }
    }

}

