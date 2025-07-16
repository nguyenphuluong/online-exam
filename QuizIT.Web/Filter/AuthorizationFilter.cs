using QuizIT.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using static QuizIT.Common.Constant;
using QuizIT.Common.Models;

namespace QuizIT.Web.Filter
{
    public class AuthorizationFilter : Attribute, IAuthorizationFilter
    {
        public bool IsAuthoriaztion { get; set; } = true;
        public int RoleID { get; set; } = Role.ALL;

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var hasClaim = filterContext.HttpContext.User.Claims.Any();

            //Kiểm tra xem đã đăng nhập chưa
            if (IsAuthoriaztion && !hasClaim)
            {
                filterContext.Result = new RedirectResult("~/");
                return;
            }

            //Lấy thông tin người đang đăng nhập từ cookie của request
           HttpHelper.HttpContext.Items["CurrentUser"] = filterContext.HttpContext.User.Claims;

            //Kiểm tra xem có quyền truy cập hay không 
            if (RoleID != Role.ALL && CurrentUser.Role != RoleID)
            {
                filterContext.Result = new RedirectResult("~/");
                return;
            }
        }
    }
}