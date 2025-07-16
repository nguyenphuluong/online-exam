using Microsoft.AspNetCore.Mvc;
using QuizIT.Web.Filter;
using static QuizIT.Common.Constant;

namespace QuizIT.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizationFilter(RoleID = Role.ADMIN)]
    public class BaseAdminController : Controller
    {
    }
}