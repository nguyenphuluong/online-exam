using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizIT.Common.Helpers
{
    /// <summary>
    /// Lớp xử lý HttpContext
    /// Mọi xử lý HttpContext sẽ được gọi tập trung tại đây
    /// </summary>
    public static class HttpHelper
    {
        private static IHttpContextAccessor _accessor;
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _accessor = httpContextAccessor;
        }

        public static HttpContext HttpContext => _accessor.HttpContext;
    }
}
