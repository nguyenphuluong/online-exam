using System;
using System.Collections.Generic;
using System.Text;

namespace QuizIT.Service.Models
{
    public class FilterHistory
    {
        public int ExamId { get; set; } = -1;
        public int UserId { get; set; } = -1;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string GetQueryString(int pageNumber, int examId, int userId)
        {

            if (pageNumber == 1)
            {
                if (examId == -1)
                {
                    return userId == -1 ? "" : $"?userId={userId}";
                }
                else
                {
                    return $"?examId={examId}" + (userId == -1 ? "" : $"&userId={userId}");
                }
            }
            else
            {
                if (examId == -1)
                {
                    return $"?pagenumber={pageNumber}" + (userId == -1 ? "" : $"&userId={userId}");
                }
                else
                {
                    return $"?pagenumber={pageNumber}&examId={examId}" + (userId == -1 ? "" : $"&userId={userId}");
                }
            }
        }
    }
}
