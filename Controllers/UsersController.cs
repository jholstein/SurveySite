using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SurveySite.Models;
using SurveySite.Models.UsersViewModels;
using Microsoft.Extensions.Logging;
using SurveySite.Services;
using Microsoft.Extensions.Options;
using SurveySite.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using SurveySite.Data.Contexts;
using SurveySite.Models.TestingViewModels;
using Microsoft.EntityFrameworkCore;

namespace SurveySite.Controllers
{
    
    public class UsersController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly TestingContext _testingContext;
        private readonly string _externalCookieScheme;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;

        public UsersController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager, 
          RoleManager<ApplicationRole> roleManager,
          TestingContext testingContext,
          IOptions<IdentityCookieOptions> identityCookieOptions,
          IEmailSender emailSender,
          ISmsSender smsSender,
          ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _testingContext = testingContext;
            _externalCookieScheme = identityCookieOptions.Value.ExternalCookieAuthenticationScheme;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<UsersController>();
        }

       



        //Users List View
        [HttpGet]
        [Authorize(Roles = "SiteAdmin,Researcher")]
        public async Task<IActionResult> Index(int sortIndex = 1)
        {  
            UserListViewModel model = new UserListViewModel();
            model.Users = new List<UserListItemViewModel>();
            List<ApplicationUser> userList = _userManager.Users.ToList();
            foreach (ApplicationUser u in userList)
            {
                var user = new UserListItemViewModel
                {
                    ListedUser = u,
                    Roles = await _userManager.GetRolesAsync(u)
                     

                };
                user.RoleString = user.Roles.FirstOrDefault();
                model.Users.Add(user);
            }

            model.SortIndex = sortIndex;
            model.RoleList = await this.GetRoleSelectList();


            switch (sortIndex)
            {
                case 1:
                    model.Users = model.Users.OrderBy(u => u.ListedUser.UserName).ToList();
                    break;
                case 2:
                    model.Users = model.Users.OrderBy(u => u.ListedUser.Id).ToList();
                    break;

                case -1:
                    model.Users = model.Users.OrderByDescending(u => u.ListedUser.UserName).ToList();
                    break;
                case -2:
                    model.Users = model.Users.OrderByDescending(u => u.ListedUser.Id).ToList();
                    break;
                    
            }


           
            return View(model);
        }



        //Allow Test for User
        [HttpPost]
        [Authorize(Roles = "SiteAdmin,Researcher")]
        public async Task<IActionResult> AllowTestForUser(UserViewModel model)
        {

            
            ApplicationUser uGet = _userManager.Users.Where(u => u.Id == model.ListedUser.Id).Include(u => u.AllowedTests).FirstOrDefault();
            Test toAllow;

            
            toAllow = _testingContext.Tests.Where(t => t.TestID == model.TestIDToAdd).FirstOrDefault();

            uGet.AllowedTests.Add(toAllow);


            

            UserListViewModel uRet = new UserListViewModel()
            {
                SelectedID = model.ListedUser.Id
            };


            _testingContext.Entry(uGet).State = EntityState.Modified;
            _testingContext.SaveChanges();
            
            return RedirectToAction(nameof(User),uRet);

        }

        
        //View User Page
        [HttpGet]
        [Authorize(Roles = "SiteAdmin,Researcher")]
        public async Task<IActionResult> User(UserListViewModel model)
        {

            _logger.LogWarning("--------------------------");
            _logger.LogWarning("---------------------------");
            _logger.LogWarning("--------------------------");
            _logger.LogWarning("---------------------------");
            ApplicationUser uGet = _testingContext.Users.Where(u => u.Id == model.SelectedID).Include(u=>u.AllowedTests).FirstOrDefault();
            
            _logger.LogWarning("Accessing user: " + model.SelectedID);

            _logger.LogWarning("--------------------------");
            _logger.LogWarning("---------------------------");
            _logger.LogWarning("--------------------------");
            _logger.LogWarning("---------------------------");

            UserViewModel uRet = new UserViewModel() {
                ListedUser = uGet,
                RoleList =await this.GetRoleSelectList(),
                Roles = await _userManager.GetRolesAsync(uGet),
                TestList = await this.GetTestSelectList()
            };
            uRet.RoleString = uRet.Roles.FirstOrDefault();
            
            return View(uRet);
        }


        //Save User Role Changes
        [HttpPost]
        [Authorize(Roles = "SiteAdmin")]
        public async Task<IActionResult> SaveUserRole(UserViewModel model)
        {
           

            ApplicationUser uGet = _userManager.Users.Where(u => u.Id == model.ListedUser.Id).FirstOrDefault();
            List<string> roleList = await this.GetRoleList();
            foreach (string roleName in roleList)
            {
                List<string> toSingleString = new List<string> ( new string[]{ roleName } );
                await _userManager.RemoveFromRolesAsync(uGet, toSingleString);

            }
            await _userManager.AddToRoleAsync(uGet, model.RoleString);



            UserListViewModel uRet = new UserListViewModel() { SelectedID = uGet.Id };

            return RedirectToAction(nameof(User), uRet);

        }




        
        



        //Create Admin Role on submission of Default Admin Form
        [HttpPost]
        public async Task<IActionResult> CreateDefaultAdmin(CreateDefaultAdminViewModel model)
        {
            if (!await _roleManager.RoleExistsAsync("SiteAdmin"))
            {
                
                var role = new ApplicationRole();
                role.Name = "SiteAdmin";
                await _roleManager.CreateAsync(role);

                var defaultAdmin = new ApplicationUser();
                defaultAdmin.UserName = model.Email;
                defaultAdmin.Email = model.Email;
                string pwd = model.Password;



                try
                {
                    await _userManager.CreateAsync(defaultAdmin, pwd);
                    await _userManager.AddToRoleAsync(defaultAdmin, "SiteAdmin");
                }
                catch
                {
                    throw new Exception();
                }

                await _signInManager.SignInAsync(defaultAdmin, isPersistent: false);
                defaultAdmin.TestsTaken = new List<Test>();
                defaultAdmin.AllowedTests = new List<Test>();
                defaultAdmin.Answers = new List<Answer>();
                return Redirect("~/");
                
            }

            return Redirect("~/");



        }

        public IActionResult CreateDefaultAdmin()
        {
            return View(new CreateDefaultAdminViewModel());


        }



        //Create Role list
        [Authorize(Roles = "SiteAdmin,Researcher")]
        public async Task<List<string>> GetRoleList()
        {
            List<string> roleList = new List<string>();

            foreach (var role in _roleManager.Roles)
            {
                roleList.Add(role.Name);
            }
            return roleList;
        }

        //Create Role SelectList
        [Authorize(Roles = "SiteAdmin,Researcher")]
        public async Task<SelectList> GetRoleSelectList()
        {
            List<RoleLister> roleList = new List<RoleLister>();

            foreach (var role in _roleManager.Roles)
            {
                RoleLister newLister = new RoleLister();
                newLister.Text = role.Name;
                newLister.Value = role.Name;
                roleList.Add(newLister);
            }
            return new SelectList(roleList, "Value", "Text");
        }

        //Create Test SelectList
        [Authorize(Roles = "SiteAdmin,Researcher")]
        public async Task<SelectList> GetTestSelectList()
        {
            List<TestLister> roleList = new List<TestLister>();


            foreach (Test test in _testingContext.Tests)
            {
                TestLister newLister = new TestLister();
                newLister.Text = test.TestName;
                newLister.Value = test.TestID;
                roleList.Add(newLister);
            }

            return new SelectList(roleList, "Value", "Text");
        }




        //SelectList Classes
        public sealed class RoleLister
        {
            public string Value { get; set; }
            public string Text { get; set; }
        }

        public sealed class TestLister
        {
            public int Value { get; set; }
            public string Text { get; set; }
        }

    }
}