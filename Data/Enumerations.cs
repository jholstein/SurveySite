using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SurveySite.Models.TestingViewModels;

namespace SurveySite.Data
{
    public class Enumerations
    {
        public static SelectList QuestionTypes = new SelectList(
            new List<QuestionTypeLister>()
            {
                new QuestionTypeLister(){Value=1, Text = "Text Box Question" },
                new QuestionTypeLister(){Value=2, Text = "Check Box Question" },
                new QuestionTypeLister(){Value=3, Text = "Other Box Question" }
            }
                , "Value", "Text");



        public static IEnumerable<QuestionTypeLister> QuestionTypesList = new List<QuestionTypeLister> {
        new QuestionTypeLister {
            Value = 1,
            Text = "Text Box Question"
        },
        new QuestionTypeLister {
            Value = 2,
            Text = "Check Box Question"
        }
        };




    }

    


    public sealed class QuestionTypeLister
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }
}
