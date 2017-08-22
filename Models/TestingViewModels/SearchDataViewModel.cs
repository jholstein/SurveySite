using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveySite.Models.TestingViewModels
{
    public class SearchDataViewModel
    {

        public List<SearchFilter> Filters { get; set; }

        public List<SearchResult> Results { get; set; }

        




        public int NewFilterType { get; set; }
        public string NewFilterValue { get; set; }


        public static IEnumerable<SearchFilter> FilterTypes = new List<SearchFilter> {
        new SearchFilter {
            FilterType = 1,
            FilterName = "Test Name Filter"
        },
        new SearchFilter {
            FilterType = 2,
            FilterName = "Question Name Filter"
        },
        new SearchFilter {
            FilterType = 3,
            FilterName = "Username Filter"
        },
        new SearchFilter {
            FilterType = 4,
            FilterName = "Answer Value Filter"
        }
        };


        public static SelectList FilterList = new SelectList(FilterTypes, "FilterType", "FilterName");



        public sealed class SearchFilter
        {
            public int FilterType { get; set; }
            public string FilterName { get; set; }
            public string FilterValue { get; set; }
        }

        public sealed class SearchResult
        {
            public string TestName { get; set; }
            public string QuestionName { get; set; }
            public string UserName { get; set; }
            public string Result { get; set; }
        }
    }


    


}
