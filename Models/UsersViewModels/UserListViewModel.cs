using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SurveySite.Models.UsersViewModels
{
    public class UserListViewModel
    {
        public string SelectedID { get; set; }
        public int SortIndex { get; set; }


        public virtual IList<UserListItemViewModel> Users { get; set; }

        public SelectList RoleList { get; set; }

        //public int NumberOfUsers { get; set; }

        //public string UserDescription { get; set; }
    }
}
