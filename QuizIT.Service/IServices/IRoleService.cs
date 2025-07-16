using QuizIT.Common.Models;
using QuizIT.Service.Entities;
using QuizIT.Service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizIT.Service.IServices
{
    public interface IRoleService
    {
        ServiceResult<Role> GetAll();
    }
}
