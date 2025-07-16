using System;
using System.Collections.Generic;
using System.Text;

namespace QuizIT.Common.Models
{
    public class PaginationModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<int> Pages { get; set; } = new List<int>();
        public int TotalPage { get; set; }
        public int MaxPageDisplay { get; set; }
    }
}
