using System;
using System.Collections.Generic;
using System.Text;

namespace QuizIT.Service.Models
{
    public class FilterCategory
    {
        public string Name { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string GetQueryString(int pageNumber, string name)
        {

            if (pageNumber == 1)
            {
                return string.IsNullOrEmpty(name) ? "" : $"?name={name}";
            }
            else
            {
                return $"?pagenumber={pageNumber}" + (string.IsNullOrEmpty(name) ? "" : $"&name={name}");
            }
        }
    }
}
