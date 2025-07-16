using QuizIT.Common.Models;
using QuizIT.Service.Entities;
using QuizIT.Service.IServices;
using QuizIT.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuizIT.Service.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly QuizITContext dbContext = new QuizITContext();
        private readonly string IMPORT_SUCCESS = "Nhập file Excel thành công";
        private readonly string CREATE_SUCCESS = "Thêm câu hỏi thành công";
        private readonly string UPDATE_SUCCESS = "Cập nhật câu hỏi thành công";
        private readonly string DELETE_SUCCESS = "Xoá câu hỏi thành công";
        private readonly string DELETE_FAILED = "Câu hỏi đã thuộc 1 bộ đề hoặc nằm trong lịch sử làm đề, không thể xoá";
        private readonly string NOT_FOUND = "Câu hỏi không tồn tại";

        public ServiceResult<Question> GetPage(FilterQuestion filter)
        {
            ServiceResult<Question> serviceResult = new ServiceResult<Question>
            {
                ResponseCode = ResponseCode.SUCCESS,
            };
            try
            {
                if (string.IsNullOrEmpty(filter.Name)) {
                    filter.Name = string.Empty;
                }
                serviceResult.Result = dbContext.Question
                    .Where(q =>
                        q.Content.ToLower().Contains(filter.Name.ToLower()) &&
                        (filter.Category == -1 || q.CategoryId == filter.Category)
                    )
                    .OrderByDescending(q => q.Id)
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();

                serviceResult.TotalRecord = dbContext.Question
                    .Where(q =>
                        q.Content.ToLower().Contains(filter.Name.ToLower()) &&
                        (filter.Category == -1 || q.CategoryId == filter.Category)
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

        public ServiceResult<Question> GetById(int questionId)
        {
            ServiceResult<Question> serviceResult = new ServiceResult<Question>
            {
                ResponseCode = ResponseCode.SUCCESS,
            };
            try
            {
                var question = dbContext.Question.FirstOrDefault(q => q.Id == questionId);
                if (question == null)
                {
                    serviceResult.ResponseCode = ResponseCode.NOT_FOUND;
                    serviceResult.ResponseMess = NOT_FOUND;
                }
                else
                {
                    serviceResult.Result.Add(question);
                }
            }
            catch
            {
                serviceResult.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                serviceResult.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }

            return serviceResult;
        }

        public async Task<ServiceResult<string>> ImportExcel(List<Question> questionLst)
        {
            ServiceResult<string> serviceResult = new ServiceResult<string>
            {
                ResponseCode = ResponseCode.SUCCESS,
                ResponseMess = IMPORT_SUCCESS
            };
            try
            {
                await dbContext.Question.AddRangeAsync(questionLst);
                await dbContext.SaveChangesAsync();
            }
            catch
            {
                serviceResult.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                serviceResult.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }

            return serviceResult;
        }

        public async Task<ServiceResult<string>> Create(Question question)
        {
            ServiceResult<string> serviceResult = new ServiceResult<string>
            {
                ResponseCode = ResponseCode.SUCCESS,
                ResponseMess = CREATE_SUCCESS
            };
            try
            {
                question.CreatedBy = CurrentUser.Id;
                dbContext.Question.Add(question);
                await dbContext.SaveChangesAsync();
            }
            catch
            {
                serviceResult.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                serviceResult.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }

            return serviceResult;
        }

        public async Task<ServiceResult<string>> Update(Question question)
        {
            ServiceResult<string> serviceResult = new ServiceResult<string>
            {
                ResponseCode = ResponseCode.SUCCESS,
                ResponseMess = UPDATE_SUCCESS
            };
            try
            {

                //Lấy ra question cũ trong db
                var questionOld = dbContext.Question.FirstOrDefault(q => q.Id == question.Id);
                //Sai id
                if (questionOld == null)
                {
                    serviceResult.ResponseCode = ResponseCode.NOT_FOUND;
                    serviceResult.ResponseMess = NOT_FOUND;
                }
                //Cập nhật lại các thông tin
                questionOld.Content = question.Content;
                questionOld.AnswerA = question.AnswerA;
                questionOld.AnswerB = question.AnswerB;
                questionOld.AnswerC = question.AnswerC;
                questionOld.AnswerD = question.AnswerD;
                questionOld.CategoryId = question.CategoryId;
                questionOld.AnswerCorrect = question.AnswerCorrect;
                dbContext.Question.Update(questionOld);
                await dbContext.SaveChangesAsync();


            }
            catch
            {
                serviceResult.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                serviceResult.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }

            return serviceResult;
        }

        public async Task<ServiceResult<string>> Delete(Question question)
        {
            ServiceResult<string> serviceResult = new ServiceResult<string>
            {
                ResponseCode = ResponseCode.SUCCESS,
                ResponseMess = DELETE_SUCCESS
            };
            try
            {
                //Kiểm tra xem câu hỏi đã thuộc bộ đề/lịch sử làm đề nào chưa nào chưa 
                if (dbContext.ExamDetail.FirstOrDefault(q => q.QuestionId == question.Id) != null)
                {
                    serviceResult.ResponseCode = ResponseCode.BAD_REQUEST;
                    serviceResult.ResponseMess = DELETE_FAILED;
                }
                else
                {
                    dbContext.Question.Remove(dbContext.Question.FirstOrDefault(q => q.Id == question.Id));
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
    }
}