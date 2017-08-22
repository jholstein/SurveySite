using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SurveySite.Models.TestingViewModels
{
    public class Answer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnswerID { get; set; }


        public int TestID { get; set; }
        public string TestName { get; set; }

        public int QuestionID { get; set; }
        public string QuestionName { get; set; }
        public int QuestionType { get; set; }
        public string QuestionAnswer { get; set; }

        public string UserName { get; set; }











    }
}
