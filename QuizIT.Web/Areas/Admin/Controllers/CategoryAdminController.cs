using Microsoft.AspNetCore.Mvc;
using QuizIT.Common.Models;
using QuizIT.Service.IServices;
using QuizIT.Service.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using QuizIT.Service.Models;
using QuizIT.Common.Helpers;

namespace QuizIT.Web.Areas.Admin.Controllers
{
    public class CategoryAdminController : BaseAdminController
    {
        private const int MODAL_ACTION_CREATE = 1;
        private const int MODAL_ACTION_UPDATE = 2;
        private readonly ICategoryService categoryService;

        public CategoryAdminController(ICategoryService categoryService, IAuthenticateService authenticateService)
        {
            this.categoryService = categoryService;
        }

        [Route("/admin/chu-de")]
        public IActionResult Index(FilterCategory filter)
        {
            ViewBag.ActivePage = "category";
            var serviceResult = categoryService.GetPage(filter);
            //Gọi service bị lỗi
            if (serviceResult.ResponseCode != ResponseCode.SUCCESS)
            {
                return Redirect("~/internal-server-error");
            }
            ViewBag.PaginationModel = PaginationHelper.GetPaginationModel(filter.PageNumber, filter.PageSize, serviceResult.TotalRecord);
            ViewBag.Filter = filter;
            return View(serviceResult.Result);
        }

        [HttpPost]
        public IActionResult ModalCategory(int action, int categoryId)
        {
            //Gọi service để lấy thông tin
            Category category = new Category();
            if (action == MODAL_ACTION_UPDATE)
            {
                var serviceResult = categoryService.GetById(categoryId);
                //Lỗi thì văng exception để ajax xử lý
                if (serviceResult.ResponseCode != ResponseCode.SUCCESS)
                {
                    throw new Exception();
                }
                category = serviceResult.Result.FirstOrDefault();
            }
            ViewBag.Action = action;
            ViewBag.CategoryId = categoryId;
            return PartialView(category);
        }

        [HttpPost]
        public async Task<IActionResult> EventCreate(Category category)
        {
            var serviceResult = await categoryService.Create(category);
            return Json(serviceResult);
        }

        [HttpPost]
        public async Task<IActionResult> EventUpdate(Category category)
        {
            var serviceResult = await categoryService.Update(category);
            return Json(serviceResult);
        }

        [HttpPost]
        public async Task<IActionResult> EventDelete(Category category)
        {
            var serviceResult = await categoryService.Delete(category);
            return Json(serviceResult);
        }
    }
}