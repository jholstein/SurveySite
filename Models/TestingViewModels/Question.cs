using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace SurveySite.Models.TestingViewModels
{
    public class Question
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuestionID { get; set; }
        public int QuestionType { get; set; }
        public string QuestionName { get; set; }
        public string QuestionAnswer { get; set; }
        //public virtual IList<Test> Tests { get; set; }

        
        public bool BooleanQuestionAnswer { get; set; }





    }
}
