using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using SurveySite.Data;
using SurveySite.Controllers;
using System.Data.SqlServerCe;
using SurveySite.Data.Contexts;

namespace SurveySite.Models.TestingViewModels
{
    public class ViewTestsViewModel
    {
        public ViewTestsViewModel(){}

        public int TestID { get; set; }

        public IList<Test> TestsList { get;set;}


       
       



    }
}
