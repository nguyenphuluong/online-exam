using System;
using System.Collections.Generic;
using System.Text;

namespace QuizIT.Common.Models
{
    public class ResponseCode
    {
        public static readonly string SUCCESS = "200";

        public static readonly string BAD_REQUEST = "400";

        public static readonly string NOT_FOUND = "404";

        public static readonly string INTERNAL_SERVER_ERROR = "500";
    }

    public class ResponseMessage
    {
       
        public static readonly string INTERNAL_SERVER_ERROR = "Máy chủ tạm thời không phản hồi, vui lòng thử lại sau";
    }
}
