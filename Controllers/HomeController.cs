using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SurveySite.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SurveySite.Models;
using Microsoft.AspNetCore.Authorization;

namespace SurveySite.Controllers
{
    public class HomeController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly string _externalCookieScheme;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;

        public HomeController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          RoleManager<ApplicationRole> roleManager,
          IOptions<IdentityCookieOptions> identityCookieOptions,
          IEmailSender emailSender,
          ISmsSender smsSender,
          ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _externalCookieScheme = identityCookieOptions.Value.ExternalCookieAuthenticationScheme;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<HomeController>();
        }




        public async Task<IActionResult> Index()
        {

            int alreadyDone = await Initialize();

            if (alreadyDone==1)
            { 
                return Redirect("~/Users/CreateDefaultAdmin");
            }
            else
            {
                return View();
            }
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }


        [Authorize(Roles = "SiteAdmin,Researcher")]
        public IActionResult ControlPanel()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }



        //Check if roles exist, create if not
        private async Task<int> Initialize()
        {
            if (!await _roleManager.RoleExistsAsync("SiteAdmin"))
            {



                if (!await _roleManager.RoleExistsAsync("Participant"))
                {
                    var pRole = new ApplicationRole();
                    pRole.Name = "Participant";
                    await _roleManager.CreateAsync(pRole);

                }


                if (!await _roleManager.RoleExistsAsync("Researcher"))
                {
                    var rRole = new ApplicationRole();
                    rRole.Name = "Researcher";
                    await _roleManager.CreateAsync(rRole);

                }




                return 1;

               
            }

            else
            {
                return 0;
            }

        }

        
    }
}
