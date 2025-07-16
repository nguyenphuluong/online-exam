using System.Collections.Generic;

namespace QuizIT.Common.Models
{
    public class ServiceResult<T>
    {
        public List<T> Result { get; set; } = new List<T>();

        public string ResponseCode { get; set; }

        public string ResponseMess { get; set; }

        public int TotalRecord { get; set; }
    }
}