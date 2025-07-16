using QuizIT.Common.Models;
using QuizIT.Service.Entities;
using QuizIT.Service.IServices;
using QuizIT.Service.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuizIT.Service.Services
{
    public class RoleService : IRoleService
    {
        private readonly QuizITContext dbContext = new QuizITContext();

        public ServiceResult<Role> GetAll()
        {
            ServiceResult<Role> serviceResult = new ServiceResult<Role>
            {
                ResponseCode = ResponseCode.SUCCESS
            };
            try
            {
                serviceResult.Result = dbContext.Role.ToList();
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
