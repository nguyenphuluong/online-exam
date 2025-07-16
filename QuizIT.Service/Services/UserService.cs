using QuizIT.Common.Helpers;
using QuizIT.Common.Models;
using QuizIT.Service.Entities;
using QuizIT.Service.IServices;
using QuizIT.Service.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuizIT.Service.Services
{
    public class UserService : IUserService
    {
        private readonly QuizITContext dbContext = new QuizITContext();
        private readonly string CREATE_SUCCESS = "Thêm người dùng thành công";
        private readonly string UPDATE_SUCCESS = "Cập nhật người dùng thành công";
        private readonly string EXISTS_USER_NAME = "Tên đăng nhập đã bị trùng, vui lòng nhập tên đăng nhập khác";
        private readonly string NOT_FOUND = "Người dùng không tồn tại";

        public ServiceResult<User> GetPage(FilterUser filter)
        {
            ServiceResult<User> serviceResult = new ServiceResult<User>
            {
                ResponseCode = ResponseCode.SUCCESS
            };
            try
            {
                if (string.IsNullOrEmpty(filter.Name))
                {
                    filter.Name = string.Empty;
                }
                serviceResult.Result = dbContext.User
                    .Where(c =>
                       (
                           c.FullName.ToLower().Contains(filter.Name.ToLower()) ||
                           c.UserName.ToLower().Contains(filter.Name.ToLower())
                       ) &&
                       (filter.Role == -1 || c.RoleId == filter.Role) &&
                       c.Id != CurrentUser.Id //Không lấy bản ghi người đang đăng nhập
                      )
                    .OrderByDescending(c => c.Id)
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();

                serviceResult.TotalRecord = dbContext.User
                    .Where(c =>
                       (
                           c.FullName.ToLower().Contains(filter.Name.ToLower()) ||
                           c.UserName.ToLower().Contains(filter.Name.ToLower())
                       ) &&
                       (filter.Role == -1 || c.RoleId == filter.Role)
                      ).Count();
            }
            catch
            {
                serviceResult.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                serviceResult.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }

            return serviceResult;
        }

        public ServiceResult<User> GetById(int userId)
        {
            ServiceResult<User> serviceResult = new ServiceResult<User>
            {
                ResponseCode = ResponseCode.SUCCESS,
            };
            try
            {
                User user = dbContext.User.FirstOrDefault(c => c.Id == userId);
                if (user == null)
                {
                    return new ServiceResult<User>
                    {
                        ResponseCode = ResponseCode.NOT_FOUND,
                    };
                }
                serviceResult.Result.Add(user);
            }
            catch
            {
                serviceResult.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                serviceResult.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }

            return serviceResult;
        }

        public async Task<ServiceResult<string>> Create(User user)
        {
            ServiceResult<string> serviceResult = new ServiceResult<string>
            {
                ResponseCode = ResponseCode.SUCCESS,
                ResponseMess = CREATE_SUCCESS
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
                        ResponseMess = EXISTS_USER_NAME
                    };
                }
                user.Password = MD5Helper.Encode(user.Password);
                await dbContext.User.AddAsync(user);
                await dbContext.SaveChangesAsync();
            }
            catch
            {
                serviceResult.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                serviceResult.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }

            return serviceResult;
        }

        public async Task<ServiceResult<string>> Update(User user)
        {

            ServiceResult<string> serviceResult = new ServiceResult<string>
            {
                ResponseCode = ResponseCode.SUCCESS,
                ResponseMess = UPDATE_SUCCESS
            };
            try
            {
                //Lấy ra user cũ trong db
                User userOld = dbContext.User.FirstOrDefault(c => c.Id == user.Id);
                //Sai id
                if (userOld == null)
                {
                    serviceResult.ResponseCode = ResponseCode.NOT_FOUND;
                    serviceResult.ResponseMess = NOT_FOUND;
                }
                //Cập nhật lại các thông tin
                userOld.RoleId = user.RoleId;
                dbContext.User.Update(userOld);
                await dbContext.SaveChangesAsync();
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