using QuizIT.Common.Models;
using QuizIT.Service.Entities;
using QuizIT.Service.Models;
using System.Threading.Tasks;

namespace QuizIT.Service.IServices
{
    public interface ICategoryService
    {
        ServiceResult<Category> GetPage(FilterCategory filter);

        ServiceResult<Category> GetById(int categoryId);

        Task<ServiceResult<string>> Create(Category category);

        Task<ServiceResult<string>> Update(Category category);

        Task<ServiceResult<string>> Delete(Category category);
    }
}