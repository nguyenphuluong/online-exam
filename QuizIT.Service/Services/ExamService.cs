using QuizIT.Common.Models;
using QuizIT.Service.Entities;
using QuizIT.Service.IServices;
using QuizIT.Service.Models;
using static QuizIT.Common.Constant.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuizIT.Service.Services
{
    public class ExamService : IExamService
    {
        private readonly QuizITContext dbContext = new QuizITContext();
        private readonly string CREATE_SUCCESS = "Thêm bộ đề thành công";
        private readonly string UPDATE_SUCCESS = "Cập nhật bộ đề thành công";
        private readonly string DELETE_SUCCESS = "Xoá bộ đề thành công";
        private readonly string DELETE_FAILED = "Bộ đề đã thuộc đã nằm trong lịch sử làm đề hoặc bảng xếp hạng, không thể xoá";
        private readonly string NOT_FOUND = "Bộ đề không tồn tại";
        private readonly string NOT_FOUND_HISTORY = "Lịch sử không tồn tại";
        private readonly string INAVTIVE = "Bộ đề không hoạt động";
        private readonly string SUBMIT_SUCCESS = "Nộp bài thành công";
        private readonly string SUBMIT_AGAIN_SUCCESS = "Chấm lại bài thành công";

        public ServiceResult<Rank> GetAllRank(int examId)
        {
            ServiceResult<Rank> serviceResult = new ServiceResult<Rank>
            {
                ResponseCode = ResponseCode.SUCCESS
            };
            try
            {
                serviceResult.Result = dbContext.Rank
                    .Where(q => q.ExamId == examId)
                    .OrderByDescending(q => q.Point)
                    .ThenBy(q => q.TimeDoExam)
                    .Take(10)
                    .ToList();
            }
            catch
            {
                serviceResult.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                serviceResult.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }

            return serviceResult;
        }

        public ServiceResult<History> GetHistoryPage(FilterHistory filter)
        {
            ServiceResult<History> serviceResult = new ServiceResult<History>
            {
                ResponseCode = ResponseCode.SUCCESS
            };
            try
            {
                //Admin thì xem và có thể lọc toàn bộ lịch sử của người dùng
                if (CurrentUser.Role == ADMIN)
                {
                    serviceResult.Result = dbContext.History
                    .OrderByDescending(q => q.CreatedAt)
                    .Where(q =>
                        (filter.UserId == -1 || q.UserId == filter.UserId) &&
                        (filter.ExamId == -1 || q.ExamId == filter.ExamId)
                    )
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();

                    serviceResult.TotalRecord = dbContext.History
                        .Where(q =>
                             (filter.UserId == -1 || q.UserId == filter.UserId) &&
                             (filter.ExamId == -1 || q.ExamId == filter.ExamId)
                         )
                         .Count();
                }
                //Khách hàng thì chỉ được xem của mình
                else
                {
                    serviceResult.Result = dbContext.History
                    .OrderByDescending(q => q.CreatedAt)
                    .Where(q =>
                        (q.UserId == CurrentUser.Id) &&
                        (filter.ExamId == -1 || q.ExamId == filter.ExamId)
                    )
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();

                    serviceResult.TotalRecord = dbContext.History
                        .Where(q =>
                             (q.UserId == CurrentUser.Id) &&
                             (filter.ExamId == -1 || q.ExamId == filter.ExamId)
                         )
                         .Count();
                }

            }
            catch
            {
                serviceResult.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                serviceResult.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }

            return serviceResult;
        }

        public ServiceResult<History> GetHistoryById(int historyId)
        {
            ServiceResult<History> serviceResult = new ServiceResult<History>
            {
                ResponseCode = ResponseCode.SUCCESS,
            };
            try
            {
                History history = dbContext.History.FirstOrDefault(c =>
                    (CurrentUser.Role == ADMIN || c.UserId == CurrentUser.Id) &&
                    c.Id == historyId
                );
                if (history == null)
                {
                    return new ServiceResult<History>
                    {
                        ResponseCode = ResponseCode.NOT_FOUND,
                    };
                }
                serviceResult.Result.Add(history);
            }
            catch
            {
                serviceResult.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                serviceResult.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }

            return serviceResult;
        }

        public ServiceResult<Exam> GetPage(FilterExam filter)
        {
            ServiceResult<Exam> serviceResult = new ServiceResult<Exam>
            {
                ResponseCode = ResponseCode.SUCCESS
            };
            try
            {
                if (string.IsNullOrEmpty(filter.Name))
                {
                    filter.Name = string.Empty;
                }
                serviceResult.Result = dbContext.Exam
                     .Where(q =>
                         q.ExamName.ToLower().Contains(filter.Name.ToLower()) &&
                         (filter.Category == -1 || q.CategoryId == filter.Category) &&
                         (filter.IsActive == -1 || q.IsActive == Convert.ToBoolean(filter.IsActive))
                     )
                     .OrderByDescending(q => q.Id)
                     .Skip((filter.PageNumber - 1) * filter.PageSize)
                     .Take(filter.PageSize)
                     .ToList();

                serviceResult.TotalRecord = dbContext.Exam
                    .Where(q =>
                         q.ExamName.ToLower().Contains(filter.Name.ToLower()) &&
                         (filter.Category == -1 || q.CategoryId == filter.Category) &&
                         (filter.IsActive == -1 || q.IsActive == Convert.ToBoolean(filter.IsActive))
                     )
                    .Count();
            }
            catch
            {
                serviceResult.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                serviceResult.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }

            return serviceResult;
        }

        public ServiceResult<Exam> GetById(int examdId)
        {
            ServiceResult<Exam> serviceResult = new ServiceResult<Exam>
            {
                ResponseCode = ResponseCode.SUCCESS,
            };
            try
            {
                Exam exam = dbContext.Exam.FirstOrDefault(c => c.Id == examdId);
                if (exam == null)
                {
                    return new ServiceResult<Exam>
                    {
                        ResponseCode = ResponseCode.NOT_FOUND,
                    };
                }
                serviceResult.Result.Add(exam);
            }
            catch
            {
                serviceResult.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                serviceResult.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }

            return serviceResult;
        }

        public async Task<ServiceResult<string>> Create(Exam exam, List<int> questionIdLst)
        {
            ServiceResult<string> serviceResult = new ServiceResult<string>
            {
                ResponseCode = ResponseCode.SUCCESS,
                ResponseMess = CREATE_SUCCESS
            };
            try
            {
                //Thêm bảng exam trước
                exam.CreateBy = CurrentUser.Id;
                await dbContext.Exam.AddAsync(exam);
                await dbContext.SaveChangesAsync();

                //Thêm bảng exam detail
                List<ExamDetail> examDetailLst = new List<ExamDetail>();
                for (int i = 0; i < questionIdLst.Count; i++)
                {
                    examDetailLst.Add(new ExamDetail
                    {
                        ExamId = exam.Id,
                        QuestionId = questionIdLst[i],
                        Order = (i + 1)
                    });
                }

                await dbContext.ExamDetail.AddRangeAsync(examDetailLst);
                await dbContext.SaveChangesAsync();

            }
            catch
            {
                serviceResult.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                serviceResult.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }

            return serviceResult;
        }

        public async Task<ServiceResult<string>> Update(Exam examNew, List<int> questionIdNewLst)
        {
            ServiceResult<string> serviceResult = new ServiceResult<string>
            {
                ResponseCode = ResponseCode.SUCCESS,
                ResponseMess = UPDATE_SUCCESS
            };
            try
            {
                //Lấy ra exam cũ trong db
                Exam examOld = dbContext.Exam.FirstOrDefault(c => c.Id == examNew.Id);
                //Sai id
                if (examOld == null)
                {
                    serviceResult.ResponseCode = ResponseCode.NOT_FOUND;
                    serviceResult.ResponseMess = NOT_FOUND;
                }
                else
                {
                    //Cập nhật thông tin exam
                    examOld.ExamName = examNew.ExamName;
                    examOld.CategoryId = examNew.CategoryId;
                    examOld.Time = examNew.Time;
                    examOld.IsActive = examNew.IsActive;
                    //Lưu lại xuống database 
                    dbContext.Exam.Update(examOld);
                    await dbContext.SaveChangesAsync();
                    //Cập nhật question của exam
                    await UpdateQuesitionOfExam(examOld.Id, examOld.ExamDetail.ToList(), questionIdNewLst);
                    //Xoá những bảng xếp có số điểm > số câu hỏi
                    dbContext.Rank.RemoveRange(dbContext.Rank.Where(r => r.Point > questionIdNewLst.Count));
                    //Xoá những bảng xếp có thời gian hoàn thành > thời gian làm của bộ đề
                    dbContext.Rank.RemoveRange(dbContext.Rank.Where(r => r.TimeDoExam > (double)examOld.Time));
                    await dbContext.SaveChangesAsync();
                }
            }
            catch
            {
                serviceResult.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                serviceResult.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }

            return serviceResult;
        }

        public async Task<ServiceResult<string>> Delete(Exam exam)
        {
            ServiceResult<string> serviceResult = new ServiceResult<string>
            {
                ResponseCode = ResponseCode.SUCCESS,
                ResponseMess = DELETE_SUCCESS
            };
            try
            {
                //Kiểm tra xem bộ đề đã có trong lịch sử/bảng xếp hạng chưa
                if (dbContext.History.FirstOrDefault(e => e.ExamId == exam.Id) != null ||
                    dbContext.Rank.FirstOrDefault(q => q.ExamId == exam.Id) != null)
                {
                    serviceResult.ResponseCode = ResponseCode.BAD_REQUEST;
                    serviceResult.ResponseMess = DELETE_FAILED;
                }
                else
                {
                    //Xoá bảng exam detail
                    dbContext.ExamDetail.RemoveRange(dbContext.ExamDetail.Where(c => c.ExamId == exam.Id));
                    //Xoá bảng exam
                    dbContext.Exam.Remove(dbContext.Exam.FirstOrDefault(c => c.Id == exam.Id));
                    await dbContext.SaveChangesAsync();
                }

            }
            catch
            {
                serviceResult.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                serviceResult.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }

            return serviceResult;
        }

        private async Task UpdateQuesitionOfExam(int examId, List<ExamDetail> examDetailldLst, List<int> questionIdNewLst)
        {
            List<ExamDetail> examDetailCreateLst = new List<ExamDetail>();
            List<ExamDetail> examDetailUpdateLst = new List<ExamDetail>();
            List<ExamDetail> examDetailDeleteLst = new List<ExamDetail>();
            //Duyệt list exam detail cũ
            foreach (var examDetail in examDetailldLst)
            {
                //Nếu nằm trong list question id mới => cập nhật exam detail
                int indexOf = questionIdNewLst.IndexOf(examDetail.QuestionId);
                if (indexOf != -1)
                {
                    examDetail.Order = indexOf + 1;
                    examDetailUpdateLst.Add(examDetail);
                }
                //Nếu không trong list question id mới cập nhật => xoá exam detail
                else
                {
                    examDetailDeleteLst.Add(examDetail);
                }
            }

            //Duyệt list question id mới 
            for (int i = 0; i < questionIdNewLst.Count; i++)
            {
                //Nếu không nằm trong list exam detail cũ => thêm mới
                if (examDetailldLst.FirstOrDefault(c => c.QuestionId == questionIdNewLst[i]) == null)
                {
                    examDetailCreateLst.Add(new ExamDetail
                    {
                        ExamId = examId,
                        QuestionId = questionIdNewLst[i],
                        Order = (i + 1)
                    });
                }
            }

            //Cập nhật
            dbContext.ExamDetail.RemoveRange(examDetailDeleteLst);
            dbContext.ExamDetail.UpdateRange(examDetailUpdateLst);
            await dbContext.ExamDetail.AddRangeAsync(examDetailCreateLst);
        }

        public async Task<ServiceResult<int>> MarkPoint(int examId, double timeDoExam, List<QuestionSelect> questionSelectLst)
        {
            ServiceResult<int> serviceResult = new ServiceResult<int>
            {
                ResponseCode = ResponseCode.SUCCESS,
                ResponseMess = SUBMIT_SUCCESS
            };
            try
            {
                Exam exam = dbContext.Exam.FirstOrDefault(c => c.Id == examId);
                //Sai id
                if (exam == null)
                {
                    return new ServiceResult<int>
                    {
                        ResponseCode = ResponseCode.NOT_FOUND,
                        ResponseMess = NOT_FOUND
                    };
                }
                //Bộ đề không hoạt động
                if (exam.IsActive == false)
                {
                    return new ServiceResult<int>
                    {
                        ResponseCode = ResponseCode.BAD_REQUEST,
                        ResponseMess = INAVTIVE
                    };
                }
                if (timeDoExam > exam.Time)
                {
                    timeDoExam = exam.Time;
                }
                int point = 0;
                List<HistoryDetail> historyDetailLst = new List<HistoryDetail>();
                //Duyệt các câu hỏi của bộ đề
                foreach (var examDetail in exam.ExamDetail.OrderBy(c => c.Order))
                {
                    //Kiểm tra xem người dùng có trả lời câu hỏi này hay k
                    var questionSelect = questionSelectLst.FirstOrDefault(c => c.QuestionId == examDetail.QuestionId);
                    //Nêu chọn đúng đáp án thì tăng điểm lên
                    if (questionSelect != null && examDetail.Question.AnswerCorrect == questionSelect.AnswerSelect)
                    {
                        point++;
                    }
                    //Add vào history detail
                    historyDetailLst.Add(new HistoryDetail
                    {
                        QuestionId = examDetail.QuestionId,
                        AnswerSelect = questionSelect == null ? "X" : questionSelect.AnswerSelect
                    });
                }

                //Lưu lịch sử
                History history = new History
                {
                    ExamId = examId,
                    UserId = CurrentUser.Id,
                    TimeDoExam = timeDoExam,
                    Point = point
                };
                dbContext.History.Add(history);
                await dbContext.SaveChangesAsync();

                //Lưu chi tiết lịch sử
                dbContext.HistoryDetail.AddRange(historyDetailLst.Select(x => new HistoryDetail
                {
                    HistoryId = history.Id, //Cập nhật historyId
                    QuestionId = x.QuestionId,
                    AnswerSelect = x.AnswerSelect
                }));
                await dbContext.SaveChangesAsync();

                //Cập nhật bảng xếp hạng
                await UpdateRank(examId, timeDoExam, point);
                //Trả ra id của history vừa thêm để điều hướng
                serviceResult.Result.Add(history.Id);
            }
            catch
            {
                serviceResult.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                serviceResult.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }
            return serviceResult;
        }

        public async Task<ServiceResult<int>> MarkPointAgain(int historyId, double timeDoExam)
        {
            ServiceResult<int> serviceResult = new ServiceResult<int>
            {
                ResponseCode = ResponseCode.SUCCESS,
                ResponseMess = SUBMIT_AGAIN_SUCCESS
            };
            try
            {
                //Lấy ra history cũ
                History historyOld = dbContext.History.FirstOrDefault(c => c.Id == historyId);
                //Sai id
                if (historyOld == null)
                {
                    return new ServiceResult<int>
                    {
                        ResponseCode = ResponseCode.NOT_FOUND,
                        ResponseMess = NOT_FOUND_HISTORY
                    };
                }
                //Bộ đề không hoạt động
                if (historyOld.Exam.IsActive == false)
                {
                    return new ServiceResult<int>
                    {
                        ResponseCode = ResponseCode.BAD_REQUEST,
                        ResponseMess = INAVTIVE
                    };
                }
                if (timeDoExam > historyOld.Exam.Time)
                {
                    timeDoExam = historyOld.Exam.Time;
                }
                int point = 0;
                //Lấy ra danh sách đáp án người dùng đã chọn
                List<HistoryDetail> historyDetailOld = historyOld.HistoryDetail.ToList();
                //Duyệt đáp án trong bộ đề
                foreach (var examDetail in historyOld.Exam.ExamDetail)
                {
                    //Kiểm tra xem người dùng có trả lời câu hỏi này hay k
                    var questionSelect = historyDetailOld.FirstOrDefault(c => c.QuestionId == examDetail.QuestionId);
                    //Nêu chọn đúng đáp án thì tăng điểm lên
                    if (questionSelect != null && examDetail.Question.AnswerCorrect == questionSelect.AnswerSelect)
                    {
                        point++;
                    }
                }

                //Update lịch sử
                historyOld.TimeDoExam = timeDoExam;
                historyOld.Point = point;
                dbContext.History.Update(historyOld);
                await dbContext.SaveChangesAsync();

                //Cập nhật bảng xếp hạng
                await UpdateRank(historyOld.ExamId, timeDoExam, point);

            }
            catch
            {
                serviceResult.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                serviceResult.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }
            return serviceResult;
        }

        private async Task UpdateRank(int examId, double timeDoExam, int point)
        {
            //Kiểm tra xem người dùng đã có trong bảng xếp hạng này hay chưa
            var rankOld = dbContext.Rank.FirstOrDefault(r => r.ExamId == examId && r.UserId == CurrentUser.Id);
            //Chưa có trong bảng xếp hạng thì thêm mới
            if (rankOld == null)
            {
                dbContext.Rank.Add(new Rank
                {
                    ExamId = examId,
                    UserId = CurrentUser.Id,
                    TimeDoExam = timeDoExam,
                    Point = point
                });
            }
            //Đã có thì kiểm tra thành tích để chỉ lưu lại thành tích tốt nhất
            else
            {
                //Điểm cũ nhỏ hơn điểm hiện tại thì lưu lại điểm mới
                if (rankOld.Point < point)
                {
                    rankOld.Point = point;
                    rankOld.TimeDoExam = timeDoExam;
                    dbContext.Rank.Update(rankOld);
                }
                //Điểm cũ bằng điểm hiện tại và thời gian cũ lâu hơn thời gian hiện tại thì lưu lại thời gian mới
                else if (rankOld.Point == point && rankOld.TimeDoExam > timeDoExam)
                {
                    rankOld.TimeDoExam = timeDoExam;
                    dbContext.Rank.Update(rankOld);
                }
            }
            await dbContext.SaveChangesAsync();
        }

        public ServiceResult<Exam> GetTopExam(int top)
        {
            ServiceResult<Exam> serviceResult = new ServiceResult<Exam>
            {
                ResponseCode = ResponseCode.SUCCESS
            };
            try
            {
                serviceResult.Result = dbContext.Exam
                     .OrderByDescending(q => q.History.Count)
                     .Take(top == -1 ? int.MaxValue : top)
                     .ToList();
            }
            catch
            {
                serviceResult.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                serviceResult.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }

            return serviceResult;
        }
    }
}
