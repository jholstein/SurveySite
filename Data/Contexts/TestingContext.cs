using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SurveySite.Data;
using SurveySite.Models.TestingViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SurveySite.Models;

namespace SurveySite.Data.Contexts
{
    public class TestingContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public TestingContext(DbContextOptions<TestingContext> options)
            : base(options)
        {
        }

        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }

        

       


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Relationship Builder
            builder.Entity<Test>().HasMany(c => c.Questions);
            //builder.Entity<Question>().HasMany(c => c.Tests);


            builder.Entity<ApplicationUser>().HasMany(c => c.AllowedTests);
            builder.Entity<ApplicationUser>().HasMany(c => c.TestsTaken);
            builder.Entity<ApplicationUser>().HasMany(c => c.Answers);

            //builder.Entity<ApplicationUser>().Ignore(x => x.LockoutEnd);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
