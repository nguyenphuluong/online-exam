using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuizIT.Common.Helpers;
using QuizIT.Common.Models;
using QuizIT.Service.Entities;
using QuizIT.Service.IServices;
using QuizIT.Service.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuizIT.Web.Controllers
{
    public class HistoryController : BaseController
    {
        private readonly IExamService examService;
        private readonly IUserService userService;

        public HistoryController(IExamService examService, IUserService userService)
        {
            this.examService = examService;
            this.userService = userService;
        }

        //Danh sách lịch sử
        [Route("/lich-su")]
        public IActionResult Index(FilterHistory filter)
        {
            ViewBag.ActivePage = "history";
            //Lấy danh sách lịch sử
            var historyServiceResult = examService.GetHistoryPage(filter);
            //Lấy danh sách bộ đề và người dùng
            var examServiceResult = examService.GetPage(new FilterExam
            {
                PageSize = int.MaxValue
            });
            var userServiceResult = userService.GetPage(new FilterUser
            {
                PageSize = int.MaxValue
            });
            //Gọi service bị lỗi
            if (historyServiceResult.ResponseCode != ResponseCode.SUCCESS ||
                examServiceResult.ResponseCode != ResponseCode.SUCCESS ||
                userServiceResult.ResponseCode != ResponseCode.SUCCESS)
            {
                return Redirect("~/internal-server-error");
            }
            ViewBag.PaginationModel = PaginationHelper.GetPaginationModel(filter.PageNumber, filter.PageSize, historyServiceResult.TotalRecord);
            ViewBag.Filter = filter;
            ViewBag.ExamLst = examServiceResult.Result;
            ViewBag.UserLst = userServiceResult.Result;
            return View(historyServiceResult.Result.OrderByDescending(c => c.CreatedAt).ToList());
        }

        //Chi tiet lich su
        [Route("/lich-su/chi-tiet/{historyId}")]
        public IActionResult Detail(int historyId)
        {
            ViewBag.ActivePage = "history";
            //Lấy thông tin lịch sử
            var historyServiceResult = examService.GetHistoryById(historyId);
            //Sai id
            if (historyServiceResult.ResponseCode == ResponseCode.NOT_FOUND)
            {
                return Redirect("~/error");
            }
            //Gọi service bị lỗi
            if (historyServiceResult.ResponseCode != ResponseCode.SUCCESS)
            {
                return Redirect("~/internal-server-error");
            }
            return View(historyServiceResult.Result.FirstOrDefault());
        }

        //Sự kiện chấm lại điểm
        [HttpPost]
        public async Task<IActionResult> EventMarkPointAgain(int historyId, double timeDoExam)
        {
            var serviceResult = await examService.MarkPointAgain(historyId, timeDoExam);
            return Json(serviceResult);
        }

        [HttpPost]
        public void EventExportHistory(int historyId)
        {

            var historyServiceResult = examService.GetHistoryById(historyId);
            //Văng lỗi ra FE để ajax xử lý
            if (historyServiceResult.ResponseCode == ResponseCode.SUCCESS)
            {
                var history = historyServiceResult.Result.FirstOrDefault();
                var lstExportHistory = GetLstExportHistory(history);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Kết quả làm bài");
                FillHeaderExcel(ref workSheet);
                FillBodyExcel(ref workSheet, lstExportHistory);
                string excelName = $"{history.User.FullName.Replace(" ", "_")}_" +
                                   $"{history.Exam.ExamName.Replace(" ", "_")}_" +
                                   $"{history.Point}_Điểm_" +
                                   $"{history.CreatedAt.ToString("dd-MM-yyyy-HH-mm")}";

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.Headers.Add("content-disposition", "attachment; filename=" + excelName + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.Body);
                    Response.Body.Flush();
                }
            }
        }

        private List<ExportHistory> GetLstExportHistory(History history)
        {
            var result = new List<ExportHistory>();
            //Duyệt danh sách đáp án đã chọn
            int order = 1;
            foreach (var historyDetail in history.HistoryDetail)
            {
                ExportHistory exportHistory = new ExportHistory
                {
                    QuestionContent = $"Câu {order}. {historyDetail.Question.Content}"
                };
                switch (historyDetail.AnswerSelect)
                {
                    case "A":
                        exportHistory.AnswerSelect = $"A. {historyDetail.Question.AnswerA}";
                        exportHistory.Result = (historyDetail.Question.AnswerCorrect == "A" ? "Đúng" : "Sai");
                        break;

                    case "B":
                        exportHistory.AnswerSelect = $"B. {historyDetail.Question.AnswerB}";
                        exportHistory.Result = (historyDetail.Question.AnswerCorrect == "B" ? "Đúng" : "Sai");
                        break;

                    case "C":
                        exportHistory.AnswerSelect = $"C. {historyDetail.Question.AnswerC}";
                        exportHistory.Result = (historyDetail.Question.AnswerCorrect == "C" ? "Đúng" : "Sai");
                        break;

                    case "D":
                        exportHistory.AnswerSelect = $"D. {historyDetail.Question.AnswerD}";
                        exportHistory.Result = (historyDetail.Question.AnswerCorrect == "D" ? "Đúng" : "Sai");
                        break;

                    default:
                        exportHistory.AnswerSelect = "Chưa chọn";
                        exportHistory.Result = "Chưa chọn";
                        break;
                }
                order++;
                result.Add(exportHistory);
            }
            return result;
        }

        private void FillHeaderExcel(ref ExcelWorksheet workSheet)
        {
            //Css cho sheet
            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;
            workSheet.Cells.Style.Font.Name = "Times New Roman";
            workSheet.Cells.Style.Font.Size = 14;
            workSheet.Column(1).Width = 100;
            workSheet.Column(2).Width = 100;
            workSheet.Column(3).Width = 20;
            //Setup tiêu đề
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Cells[1, 1].Value = "Câu hỏi";
            workSheet.Cells[1, 2].Value = "Đáp án đã chọn";
            workSheet.Cells[1, 3].Value = "Kết quả";
        }

        private void FillBodyExcel(ref ExcelWorksheet workSheet, List<ExportHistory> lstExportHistory)
        {
            int recordIndex = 2; //Bắt đầu từ 2 vì 1 là tiêu đề
            foreach (var exportHistory in lstExportHistory)
            {
                workSheet.Cells[recordIndex, 1].Value = exportHistory.QuestionContent;
                workSheet.Cells[recordIndex, 2].Value = exportHistory.AnswerSelect;
                workSheet.Cells[recordIndex, 3].Value = exportHistory.Result;
                recordIndex++;
            }
        }
    }
}