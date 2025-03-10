﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TadrousManassa.Models;

namespace TadrousManassa.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StudentLecture>()
                .HasIndex(sl => sl.Code)
                .IsUnique();
            modelBuilder.Entity<StudentLecture>()
                .HasOne(sl => sl.Student)
                .WithMany(s => s.StudentLectures)
                .HasForeignKey(sl => sl.StudentId)
                .IsRequired(false);

            modelBuilder.Entity<StudentLecture>()
                .HasOne(sl => sl.Lecture)
                .WithMany(l => l.StudentLectures)
                .HasForeignKey(sl => sl.LectureId)
                .IsRequired();

            modelBuilder.Entity<Student>()
                .HasOne(s => s.ApplicationUser)
                .WithOne(a => a.Student)
                .HasForeignKey<Student>(s => s.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Lecture> Lectures { get; set; }
        public virtual DbSet<StudentLecture> StudentLectures { get; set; }
    }
}
