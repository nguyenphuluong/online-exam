using Microsoft.AspNetCore.Mvc;
using QuizIT.Common.Models;
using QuizIT.Service.IServices;
using QuizIT.Service.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using QuizIT.Service.Models;
using QuizIT.Common.Helpers;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;

namespace QuizIT.Web.Areas.Admin.Controllers
{
    public class QuestionAdminController : BaseAdminController
    {
        private const int MODAL_ACTION_CREATE = 1;
        private const int MODAL_ACTION_UPDATE = 2;
        private readonly IQuestionService questionService;
        private readonly ICategoryService categoryService;

        public QuestionAdminController(IQuestionService questionService, ICategoryService categoryService)
        {
            this.questionService = questionService;
            this.categoryService = categoryService;
        }

        [Route("/admin/cau-hoi")]
        public IActionResult Index(FilterQuestion filter)
        {
            ViewBag.ActivePage = "question";
            //Lấy ra tất cả thể loại để filter
            var categoryServiceResult = categoryService.GetPage(new FilterCategory
            {
                PageSize = int.MaxValue
            });
            //Lấy danh sách câu hỏi
            var questionServiceResult = questionService.GetPage(filter);
            //Gọi service bị lỗi
            if (categoryServiceResult.ResponseCode != ResponseCode.SUCCESS ||
                questionServiceResult.ResponseCode != ResponseCode.SUCCESS)
            {
                return Redirect("~/internal-server-error");
            }
            ViewBag.PaginationModel = PaginationHelper.GetPaginationModel(filter.PageNumber, filter.PageSize, questionServiceResult.TotalRecord);
            ViewBag.Filter = filter;
            ViewBag.CategoryLst = categoryServiceResult.Result;
            return View(questionServiceResult.Result);
        }

        [Route("/admin/cau-hoi/tao")]
        public IActionResult Create()
        {
            ViewBag.ActivePage = "question";
            //Lấy ra tất cả thể loại để filter
            var categoryServiceResult = categoryService.GetPage(new FilterCategory
            {
                PageSize = int.MaxValue
            });
            //Gọi service bị lỗi
            if (categoryServiceResult.ResponseCode != ResponseCode.SUCCESS)

            {
                return Redirect("~/internal-server-error");
            }
            ViewBag.CategoryLst = categoryServiceResult.Result;
            return View();
        }

        [Route("/admin/cau-hoi/chi-tiet/{questionId}")]
        public IActionResult Detail(int questionId)
        {
            ViewBag.ActivePage = "question";
            //Lấy ra tất cả thể loại để filter
            var categoryServiceResult = categoryService.GetPage(new FilterCategory
            {
                PageSize = int.MaxValue
            });
            //Lấy chi tiết câu hỏi
            var questionServiceResult = questionService.GetById(questionId);
            //Sai id
            if (questionServiceResult.ResponseCode == ResponseCode.NOT_FOUND)
            {
                return Redirect("~/error");
            }
            //Gọi service bị lỗi
            if (categoryServiceResult.ResponseCode != ResponseCode.SUCCESS ||
                questionServiceResult.ResponseCode != ResponseCode.SUCCESS)
            {
                return Redirect("~/internal-server-error");
            }
            ViewBag.CategoryLst = categoryServiceResult.Result;
            return View(questionServiceResult.Result.FirstOrDefault());
        }

        [HttpPost]
        public async Task<ActionResult> EventImportExcel(int categoryId, IFormFile fileExcel)
        {
            List<Question> questionLst = new List<Question>();
            //Load data từ excel
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = new MemoryStream())
            {
                fileExcel.CopyTo(stream);
                stream.Position = 0;
                using (var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream))
                {
                    int row = 1;
                    while (reader.Read())
                    {
                        //Dữ liệu bắt đầu từ dòng 2 vì dòng 1 là của tiêu đề 
                        if (row >= 2)
                        {   
                            //Hết dữ liệu để đọc
                            if(reader.GetValue(0) == null)
                            {
                                break;
                            }
                            questionLst.Add(new Question
                            {
                                CategoryId = categoryId,
                                Content = reader.GetValue(0).ToString().Trim(),
                                AnswerA = reader.GetValue(1).ToString().Trim(),
                                AnswerB = reader.GetValue(2).ToString().Trim(),
                                AnswerC = reader.GetValue(3).ToString().Trim(),
                                AnswerD = reader.GetValue(4).ToString().Trim(),
                                AnswerCorrect = reader.GetValue(5).ToString().Trim(),
                                CreatedBy = CurrentUser.Id
                            });
                        }
                        row++;
                    }
                }
            }
            return Json(await questionService.ImportExcel(questionLst));
        }

        [HttpPost]
        public async Task<IActionResult> EventCreate(Question question)
        {
            var serviceResult = await questionService.Create(question);
            return Json(serviceResult);
        }

        [HttpPost]
        public async Task<IActionResult> EventUpdate(Question question)
        {
            var serviceResult = await questionService.Update(question);
            return Json(serviceResult);
        }

        [HttpPost]
        public async Task<IActionResult> EventDelete(Question question)
        {
            var serviceResult = await questionService.Delete(question);
            return Json(serviceResult);
        }
    }
}