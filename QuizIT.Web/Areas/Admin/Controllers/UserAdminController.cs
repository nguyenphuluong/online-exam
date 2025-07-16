using Microsoft.AspNetCore.Mvc;
using QuizIT.Common.Helpers;
using QuizIT.Common.Models;
using QuizIT.Service.Entities;
using QuizIT.Service.IServices;
using QuizIT.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuizIT.Web.Areas.Admin.Controllers
{
    public class UserAdminController : BaseAdminController
    {
        private const int MODAL_ACTION_CREATE = 1;
        private const int MODAL_ACTION_UPDATE = 2;
        private readonly IRoleService roleService;
        private readonly IUserService userService;

        public UserAdminController(IRoleService roleService, IUserService userService)
        {
            this.roleService = roleService;
            this.userService = userService;
        }

        [Route("/admin/nguoi-dung")]
        public IActionResult Index(FilterUser filter)
        {
            ViewBag.ActivePage = "user";
            //Lấy tất cả role
            var roleServiceResult = roleService.GetAll();
            //Lấy danh sách người dùng
            var userServiceResult = userService.GetPage(filter);
            //Gọi service bị lỗi
            if (roleServiceResult.ResponseCode != ResponseCode.SUCCESS ||
                userServiceResult.ResponseCode != ResponseCode.SUCCESS)
            {
                return Redirect("~/internal-server-error");
            }
            ViewBag.PaginationModel = PaginationHelper.GetPaginationModel(filter.PageNumber, filter.PageSize, userServiceResult.TotalRecord);
            ViewBag.RoleLst = roleServiceResult.Result;
            ViewBag.Filter = filter;
            return View(userServiceResult.Result);
        }

        [HttpPost]
        public IActionResult ModalUser(int action, int userId)
        {
            //Lấy tất cả role
            var roleServiceResult = roleService.GetAll();
            if (roleServiceResult.ResponseCode != ResponseCode.SUCCESS
             )
            {
                return Redirect("~/internal-server-error");
            }
            //Gọi service để lấy thông tin
            User user = new User();
            if (action == MODAL_ACTION_UPDATE)
            {
                var userServiceResult = userService.GetById(userId);
                //Lỗi thì văng exception để ajax xử lý
                if (userServiceResult.ResponseCode != ResponseCode.SUCCESS)
                {
                    throw new Exception();
                }
                user = userServiceResult.Result.FirstOrDefault();
            }
            ViewBag.RoleLst = roleServiceResult.Result;
            ViewBag.Action = action;
            ViewBag.UserId = userId;
            return PartialView(user);
        }

        [HttpPost]
        public async Task<IActionResult> EventCreate(User user)
        {
            var serviceResult = await userService.Create(user);
            return Json(serviceResult);
        }

        [HttpPost]
        public async Task<IActionResult> EventUpdate(User user)
        {
            var serviceResult = await userService.Update(user);
            return Json(serviceResult);
        }
    }
}
