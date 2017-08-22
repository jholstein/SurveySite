using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SurveySite.Models.TestingViewModels;

namespace SurveySite.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public virtual IList<Test> AllowedTests { get; set; }
        public virtual IList<Test> TestsTaken { get; set; }

        public virtual IList<Answer> Answers { get; set; }
    }
}
