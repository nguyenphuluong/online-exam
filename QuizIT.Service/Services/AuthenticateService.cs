using QuizIT.Common;
using QuizIT.Common.Helpers;
using QuizIT.Common.Models;
using QuizIT.Service.IServices;
using QuizIT.Service.Entities;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace QuizIT.Service.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly QuizITContext dbContext = new QuizITContext();
        private readonly string LOGIN_SUCCESS_MESS = "Chào mừng";
        private readonly string LOGIN_FAILED_MESS = "Sai thông tin đăng nhập";
        private readonly string REGISTRY_SUCCESS_MESS = "Đăng ký thành công";
        private readonly string REGISTRY_EXISTS_USER_NAME = "Tên đăng nhập đã bị trùng, vui lòng nhập tên đăng nhập khác";

        public async Task<ServiceResult<User>> Login(string userName, string password)
        {
            ServiceResult<User> resultService = new ServiceResult<User>
            {
                ResponseCode = ResponseCode.SUCCESS,
                ResponseMess = LOGIN_SUCCESS_MESS
            };
            try
            {
                User user = await dbContext.User
                     .FirstOrDefaultAsync(u =>
                         u.UserName.ToLower() == userName.ToLower() &&
                         u.Password == MD5Helper.Encode(password)
                      );
                if (user == null)
                {
                    return new ServiceResult<User>
                    {
                        ResponseCode = ResponseCode.BAD_REQUEST,
                        ResponseMess = LOGIN_FAILED_MESS
                    };
                }
                resultService.Result.Add(user);
            }
            catch
            {
                resultService.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                resultService.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }

            return resultService;
        }

        public async Task<ServiceResult<string>> Registry(User user)
        {
            ServiceResult<string> resultService = new ServiceResult<string>
            {
                ResponseCode = ResponseCode.SUCCESS,
                ResponseMess = REGISTRY_SUCCESS_MESS
            };
            try
            {
                //Kiểm tra userName đã tồn tại trong DB hay chưa
                bool isExistsUserName = (dbContext.User
                    .FirstOrDefault(u => u.UserName.ToLower() == user.UserName.ToLower()) != null);
                if (isExistsUserName)
                {
                    return new ServiceResult<string>
                    {
                        ResponseCode = ResponseCode.BAD_REQUEST,
                        ResponseMess = REGISTRY_EXISTS_USER_NAME
                    };
                }
                user.RoleId = Constant.Role.CLIENT;
                user.Password = MD5Helper.Encode(user.Password);
                await dbContext.User.AddAsync(user);
                await dbContext.SaveChangesAsync();
            }
            catch
            {
                resultService.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                resultService.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }

            return resultService;
        }
    }
}