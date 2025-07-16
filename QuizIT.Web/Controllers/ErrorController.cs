using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuizIT.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/error")]
        public IActionResult Error()
        {
            return View();
        }

        [Route("/forbidden")]
        public IActionResult Forbidden()
        {
            return View();
        }


        [Route("/internal-server-error")]
        public IActionResult InternalServerError()
        {
            return View();
        }
    }
}
