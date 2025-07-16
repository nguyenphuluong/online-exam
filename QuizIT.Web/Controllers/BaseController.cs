using Microsoft.AspNetCore.Mvc;
using QuizIT.Web.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuizIT.Web.Controllers
{
    [AuthorizationFilter]
    public class BaseController : Controller
    {
      
    }
}
