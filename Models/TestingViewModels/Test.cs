using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using SurveySite.Data;
using SurveySite.Controllers;
using System.Data.SqlServerCe;
using SurveySite.Models.TestingViewModels;
using SurveySite.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace SurveySite.Models.TestingViewModels
{
    public class Test
    {
        public Test(){}

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TestID { get; set; }
        public string TestName { get; set; }


        //[ForeignKey("Questions")]
        public virtual IList<Question> Questions{get; set;}



        

        //public virtual List<Question> QuestionsList { get { /return this.Questions.ToList(); } set { } }

        //public Test(int tID)
        //{
        //    this.TestID = tID;
        //    using (var context = new TestingContext())
        //    {
        //        Test thisTest = context.Tests.Where(t => t.TestID == tID).Include(t => t.Questions).FirstOrDefault();
        //        this.Questions = 
        //    }
        //}



    }
}
