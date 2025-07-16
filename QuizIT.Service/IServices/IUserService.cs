using QuizIT.Common.Models;
using QuizIT.Service.Entities;
using QuizIT.Service.Models;
using System.Threading.Tasks;

namespace QuizIT.Service.IServices
{
    public interface IUserService
    {
        ServiceResult<User> GetPage(FilterUser filter);

        ServiceResult<User> GetById(int userId);

        Task<ServiceResult<string>> Create(User user);

        Task<ServiceResult<string>> Update(User user);
    }
}
