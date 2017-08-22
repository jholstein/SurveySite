using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using SurveySite.Data;
using SurveySite.Controllers;
using System.Data.SqlServerCe;

namespace SurveySite.Models.TestingViewModels
{
    public class CreateTestViewModel
    {
        public CreateTestViewModel(){}

        
        public int TestID { get; set; }
        public string TestName { get; set; }
        
        public List<Question> Questions{ get; set; }





    }
}
