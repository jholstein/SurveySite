using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SurveySite.Data;
using SurveySite.Models.TestingViewModels;
using SurveySite.Models.ManageViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SurveySite.Data.Contexts;
using System.Data.Entity.Migrations;
using SurveySite.Models;
using SurveySite.Services;
using Microsoft.AspNetCore.Authorization;
using System.Data.SqlClient;

namespace SurveySite.Controllers
{
    
    public class TestingController : Controller
    {
        
        

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly TestingContext _testingContext;
        private readonly string _externalCookieScheme;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;

        

        public TestingController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          TestingContext testingContext,
          IOptions<IdentityCookieOptions> identityCookieOptions,
          IEmailSender emailSender,
          ISmsSender smsSender,
          ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _testingContext = testingContext;
            _externalCookieScheme = identityCookieOptions.Value.ExternalCookieAuthenticationScheme;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<TestingController>();
        }

        //POST: /Testing/DebugAction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebugAction(Models.TestingViewModels.Test model)
        {

            Test debugTest = new Test();
            debugTest.TestName = "Debug Test";


            Question debugQuestion1 = new Question() { QuestionName = "Debug Question 1", QuestionType = 1 };
            Question debugQuestion2 = new Question() { QuestionName = "Debug Question 2", QuestionType = 2 };


            debugTest.Questions = new List<Question>();

            debugTest.Questions.Add(debugQuestion1);
            debugTest.Questions.Add(debugQuestion2);
            _testingContext.Tests.Add(debugTest);
            _testingContext.SaveChanges();






            return View();

        }

        public Test LoadTest(int tID)
        {
            return _testingContext.Tests.AsNoTracking().Where(t => t.TestID == tID).AsNoTracking().Include(t => t.Questions).AsNoTracking().FirstOrDefault();
        }



        public IActionResult SearchData()
        {
            SearchDataViewModel model = new SearchDataViewModel();
            model.Results = new List<SearchDataViewModel.SearchResult>();
            model.Filters = new List<SearchDataViewModel.SearchFilter>();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> SearchData(SearchDataViewModel model)
        {
            //Create Result List
            model.Results = new List<SearchDataViewModel.SearchResult>();

            if(model.Filters==null)
            {
                model.Filters = new List<SearchDataViewModel.SearchFilter>();
            }



            switch (model.NewFilterType)
            {
                case 1:
                    model.Filters.Add(new SearchDataViewModel.SearchFilter() { FilterName = "Test Name Filter", FilterType=model.NewFilterType,FilterValue=model.NewFilterValue});
                    break;
                case 2:
                    model.Filters.Add(new SearchDataViewModel.SearchFilter() { FilterName = "Question Name Filter", FilterType = model.NewFilterType, FilterValue = model.NewFilterValue });
                    break;
                case 3:
                    model.Filters.Add(new SearchDataViewModel.SearchFilter() { FilterName = "Username Filter", FilterType = model.NewFilterType, FilterValue = model.NewFilterValue });
                    break;
                case 4:
                    model.Filters.Add(new SearchDataViewModel.SearchFilter() { FilterName = "Answer Value Filter", FilterType = model.NewFilterType, FilterValue = model.NewFilterValue });
                    break;
            }


            //Progressive Filter Filtering
            bool firstFilter = true;
            foreach (SearchDataViewModel.SearchFilter filter in model.Filters)
            {

                if (firstFilter)
                {
                    model.Results = FilterFirstResults(filter.FilterType, filter.FilterValue);
                    firstFilter = false;
                }
                else
                {
                    model.Results = FilterResults(model.Results, filter.FilterType, filter.FilterValue);
                }
            }

            
            model.NewFilterValue = "";

            return View(model);




        }


        public List<SearchDataViewModel.SearchResult> FilterResults(List<SearchDataViewModel.SearchResult> currentResults,int filterType, string filterString)
        {
            
            List<SearchDataViewModel.SearchResult> retList = new List<SearchDataViewModel.SearchResult>();

            switch (filterType)
            {
                //Testname
                case 1:
                    retList = currentResults.Where(a => a.TestName.Contains(filterString)).ToList();
                    break;

                //Question name
                case 2:
                    retList = currentResults.Where(a => a.QuestionName.Contains(filterString)).ToList();
                    break;

                //User name
                case 3:
                    retList = currentResults.Where(a => a.UserName.Contains(filterString)).ToList();
                    break;

                //Answer Value
                case 4:
                    retList = currentResults.Where(a => a.Result.Contains(filterString)).ToList();
                    break;
            }

            return retList;

        }


        public List<SearchDataViewModel.SearchResult> FilterFirstResults(int filterType, string filterString)
        {
            
            List<Answer> getList = new List<Answer>();

            switch (filterType)
            {
                //Testname
                case 1:
                    getList = _testingContext.Answers.Where(a => a.TestName.Contains(filterString)).ToList();
                    break;

                //Question name
                case 2:
                    getList = _testingContext.Answers.Where(a => a.QuestionName.Contains(filterString)).ToList();
                    break;

                //User name
                case 3:
                    getList = _testingContext.Answers.Where(a => a.UserName.Contains(filterString)).ToList();
                    break;

                //Answer Value
                case 4:
                    getList = _testingContext.Answers.Where(a => a.QuestionAnswer.Contains(filterString)).ToList();
                    break;
            }

            return AnswerToResultList(getList);

        }

        public List<SearchDataViewModel.SearchResult> AnswerToResultList(List<Answer> aList)
        {
            List<SearchDataViewModel.SearchResult> retList = new List<SearchDataViewModel.SearchResult>();

            foreach(Answer a in aList)
            {
                retList.Add(new SearchDataViewModel.SearchResult()
                {
                    TestName = a.TestName,
                    QuestionName = a.QuestionName,
                    UserName = a.UserName,
                    Result = a.QuestionAnswer
                });
            }

            return retList;
        }


        public async Task<bool> IsTestAllowed(int TestID)
        {
            ApplicationUser user = await GetCurrentUserAsync();
            string userID = await _userManager.GetUserIdAsync(user);

            user = _testingContext.Users.Where(u => u.Id == userID).Include(u => u.AllowedTests).FirstOrDefault();

            if (await _userManager.IsInRoleAsync(user, "SiteAdmin") || await _userManager.IsInRoleAsync(user, "Researcher"))
            {
                return true;
            }

            foreach (Test t in user.AllowedTests)
            {
                if (t.TestID==TestID)
                {
                    return true;
                }
            }

            return false;
        }
       

        //
        // POST: /Testing/Test
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Test(ViewTestsViewModel model)
        {

                

            if (await this.IsTestAllowed(model.TestID))
            {
                return View(this.LoadTest(model.TestID));
            }
            else
            {
                return Redirect("~/Account/AccessDenied");
            }
                
            

        }

        [Authorize]
        public IActionResult Test(Test model)
        {
            
            
                return View(this.LoadTest(model.TestID));
           

        }


        //
        // POST: /Testing/SubmitTestData
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> SubmitTestData(Test model)
        {

            bool hasTest = false;

            ApplicationUser user = await GetCurrentUserAsync();
            string userID = await _userManager.GetUserIdAsync(user);
            user = _testingContext.Users.Where(u => u.Id == userID).Include(u => u.AllowedTests).Include(u => u.TestsTaken).Include(u => u.Answers).FirstOrDefault();
            Test toAllow = _testingContext.Tests.Where(t => t.TestID == model.TestID).FirstOrDefault();

            foreach (Test t in user.AllowedTests)
            {
                if (t.TestID == toAllow.TestID)
                {
                    hasTest = true;
                }
            }

            if (hasTest==false)
            {
                return View();
            }



            Answer newAnswer;
            foreach (Question q in model.Questions)
            {
                if(q.QuestionType == 2)
                {
                    newAnswer = new Answer() { QuestionID = q.QuestionID, QuestionName = q.QuestionName, QuestionAnswer = q.BooleanQuestionAnswer.ToString(), QuestionType = q.QuestionType, TestID = model.TestID, TestName=model.TestName, UserName=user.UserName };
                }
                else
                {
                    newAnswer = new Answer() { QuestionID = q.QuestionID, QuestionName = q.QuestionName, QuestionAnswer = q.QuestionAnswer, QuestionType = q.QuestionType, TestID = model.TestID, TestName = model.TestName, UserName = user.UserName };
                }


                user.Answers.Add(newAnswer);
                _testingContext.Answers.Add(newAnswer);
            }


            
            
            user.TestsTaken.Add(toAllow);
            user.AllowedTests.Remove(toAllow);

            _testingContext.SaveChanges();
            return View(this.LoadTest(model.TestID));

        }


        //
        // POST: /Testing/MakeQuestion
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SiteAdmin,Researcher")]
        public async Task<IActionResult> MakeQuestion(Test model)
        {
            
            Question newQuestion = new Question() { QuestionName = "DEFAULT QUESTION NAME", QuestionType = 1 };
                

            Test toEdit = this.LoadTest(model.TestID);
            _testingContext.Tests.Attach(toEdit);

                
            toEdit.Questions.Add(newQuestion);
            _testingContext.Entry(toEdit).State = EntityState.Modified;
            _testingContext.SaveChanges();

            return RedirectToAction(nameof(Test), new Test() { TestID = toEdit.TestID });
        }

        //
        // POST: /Testing/EditTestData
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SiteAdmin,Researcher")]
        public async Task<IActionResult> EditTestData(Test model)
        {
            
            foreach (Question q in model.Questions)
            {
                Question oldQuestion = _testingContext.Questions.SingleOrDefault(e => e.QuestionID == q.QuestionID);
                
                oldQuestion.QuestionName=q.QuestionName;
                oldQuestion.QuestionType = q.QuestionType;
                oldQuestion.QuestionAnswer = q.QuestionAnswer;
                oldQuestion.BooleanQuestionAnswer = q.BooleanQuestionAnswer;
                

                
                _testingContext.Entry(oldQuestion).State = EntityState.Modified;
                
                _testingContext.SaveChanges();


            }


            return RedirectToAction(nameof(Test), model);

        }

        //
        // POST: /Testing/CreateTest
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SiteAdmin,Researcher")]
        public async Task<IActionResult> CreateTest(Models.TestingViewModels.Test model)
        {

            
                
                model.Questions = new List<Question>();
                _testingContext.Tests.Add(model);
                _testingContext.SaveChanges();
                


            


            return RedirectToAction("Test",model);

        }

        [Authorize(Roles = "SiteAdmin,Researcher")]
        public IActionResult CreateTest()
        {
            



            return base.View(new Test());
        }


        [Authorize(Roles = "SiteAdmin,Researcher")]
        public IActionResult ViewTests()
        {

            ViewTestsViewModel retModel = new ViewTestsViewModel();
            retModel.TestsList = (from t in _testingContext.Tests.AsNoTracking()
                              select t).ToList();


            return View(retModel);
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await GetCurrentUserAsync();
            string userID = await _userManager.GetUserIdAsync(user);
            user = _testingContext.Users.Where(u => u.Id == userID).Include(u => u.AllowedTests).FirstOrDefault();

            ViewTestsViewModel retModel = new ViewTestsViewModel();
            retModel.TestsList = user.AllowedTests;


            return View(retModel);
        }

        public IActionResult Error()
        {
            return View();
        }

        

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }


    }
}
