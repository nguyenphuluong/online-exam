using System;
using System.Collections.Generic;
using System.Text;
using static QuizIT.Common.Constant.Role;

namespace QuizIT.Service.Models
{
    public class FilterUser
    {
        public string Name { get; set; } = string.Empty;
        public int Role { get; set; } = ALL;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string GetQueryString(int pageNumber, string name, int role)
        {

            if (pageNumber == 1)
            {
                if (role == -1)
                {
                    return string.IsNullOrEmpty(name) ? "" : $"?name={name}";
                }
                else
                {
                    return $"?role={role}" + (string.IsNullOrEmpty(name) ? "" : $"&name={name}");
                }
            }
            else
            {
                if (role == -1)
                {
                    return $"?pagenumber={pageNumber}" + (string.IsNullOrEmpty(name) ? "" : $"&name={name}");
                }
                else
                {
                    return $"?pagenumber={pageNumber}&role={role}" + (string.IsNullOrEmpty(name) ? "" : $"&name={name}");
                }
            }
        }
    }
}
