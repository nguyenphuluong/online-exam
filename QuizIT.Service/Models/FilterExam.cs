using System;
using System.Collections.Generic;
using System.Text;

namespace QuizIT.Service.Models
{
    public class FilterExam
    {
        public string Name { get; set; } = string.Empty;
        public int Category { get; set; } = -1;
        public int IsActive { get; set; } = -1;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string GetQueryString(int pageNumber, string name, int category)
        {

            if (pageNumber == 1)
            {
                if (category == -1)
                {
                    return string.IsNullOrEmpty(name) ? "" : $"?name={name}";
                }
                else
                {
                    return $"?category={category}" + (string.IsNullOrEmpty(name) ? "" : $"&name={name}");
                }
            }
            else
            {
                if (category == -1)
                {
                    return $"?pagenumber={pageNumber}" + (string.IsNullOrEmpty(name) ? "" : $"&name={name}");
                }
                else
                {
                    return $"?pagenumber={pageNumber}&category={category}" + (string.IsNullOrEmpty(name) ? "" : $"&name={name}");
                }
            }
        }
    }
}
