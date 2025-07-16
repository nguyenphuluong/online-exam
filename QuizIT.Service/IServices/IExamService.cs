using QuizIT.Common.Models;
using QuizIT.Service.Entities;
using QuizIT.Service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizIT.Service.IServices
{
    public interface IExamService
    {
        ServiceResult<Exam> GetTopExam(int top);
        ServiceResult<Rank> GetAllRank(int examId);
        ServiceResult<History> GetHistoryPage(FilterHistory filter);
        ServiceResult<Exam> GetPage(FilterExam filter);
        ServiceResult<Exam> GetById(int examdId);
        Task<ServiceResult<string>> Create(Exam exam, List<int> questionIdLst);
        Task<ServiceResult<string>> Update(Exam examNew, List<int> questionIdNewLst);
        Task<ServiceResult<string>> Delete(Exam exam);
        Task<ServiceResult<int>> MarkPoint(int examId, double timeDoExam, List<QuestionSelect> questionSelectLst);
        Task<ServiceResult<int>> MarkPointAgain(int historyId, double timeDoExam);
        ServiceResult<History> GetHistoryById(int historyId);
    }
}
