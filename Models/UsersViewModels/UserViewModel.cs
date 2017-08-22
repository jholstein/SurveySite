using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SurveySite.Models.UsersViewModels
{
    public class UserViewModel
    {
        //public string UserListItemViewModelID { get; set; }
        //public string UserName { get; set; }
        //public bool EmailConfirmed { get; set; }
        public IList<string> Roles { get; set; }
        //public string Roles { get; set; }


        public ApplicationUser ListedUser { get; set; }

        public string RoleString { get; set; }

        public SelectList RoleList { get; set; }
        public SelectList TestList { get; set; }

        public int TestIDToAdd { get; set; }

        //public int NumberOfUsers { get; set; }

        //public string UserDescription { get; set; }
    }
}
