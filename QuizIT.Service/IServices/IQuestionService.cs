using QuizIT.Common.Models;
using QuizIT.Service.Entities;
using QuizIT.Service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizIT.Service.IServices
{
    public interface IQuestionService
    {
        Task<ServiceResult<string>> ImportExcel(List<Question> questionLst);

        ServiceResult<Question> GetPage(FilterQuestion filter);

        ServiceResult<Question> GetById(int questionId);

        Task<ServiceResult<string>> Create(Question question);

        Task<ServiceResult<string>> Update(Question question);

        Task<ServiceResult<string>> Delete(Question question);
    }
}