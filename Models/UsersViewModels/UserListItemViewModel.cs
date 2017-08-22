using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SurveySite.Models.UsersViewModels
{
    public class UserListItemViewModel
    {
        //public string UserListItemViewModelID { get; set; }
        //public string UserName { get; set; }
        //public bool EmailConfirmed { get; set; }
        public IList<string> Roles { get; set; }
        //public string Roles { get; set; }


        public ApplicationUser ListedUser { get; set; }


        public string RoleString { get; set; }

        //public int NumberOfUsers { get; set; }

        //public string UserDescription { get; set; }
    }
}
