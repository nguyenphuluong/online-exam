using QuizIT.Common.Models;
using QuizIT.Service.Entities;
using QuizIT.Service.IServices;
using QuizIT.Service.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuizIT.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly QuizITContext dbContext = new QuizITContext();
        private readonly string CREATE_SUCCESS = "Thêm chủ đề thành công";
        private readonly string UPDATE_SUCCESS = "Cập nhật chủ đề thành công";
        private readonly string DELETE_SUCCESS = "Xoá chủ đề thành công";
        private readonly string DELETE_FAILED = "Đã có bộ đề hoặc câu hỏi thuộc chủ đề này, không thể xoá";
        private readonly string EXISTS_CATEGORY_NAME = "Tên chủ đề đã trùng, vui lòng nhập tên khác";
        private readonly string NOT_FOUND = "Chủ đề không tồn tại";

        public ServiceResult<Category> GetPage(FilterCategory filter)
        {
            ServiceResult<Category> serviceResult = new ServiceResult<Category>
            {
                ResponseCode = ResponseCode.SUCCESS
            };
            try
            {
                if (string.IsNullOrEmpty(filter.Name))
                {
                    filter.Name = string.Empty;
                }
                serviceResult.Result = dbContext.Category
                    .Where(c => c.CategoryName.ToLower().Contains(filter.Name.ToLower()))
                    .OrderByDescending(c => c.Id)
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();
                serviceResult.TotalRecord = dbContext.Category
                    .Where(c => c.CategoryName.ToLower().Contains(filter.Name.ToLower())).Count();
            }
            catch
            {
                serviceResult.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                serviceResult.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }

            return serviceResult;
        }

        public ServiceResult<Category> GetById(int categoryId)
        {
            ServiceResult<Category> serviceResult = new ServiceResult<Category>
            {
                ResponseCode = ResponseCode.SUCCESS,
            };
            try
            {
                Category category = dbContext.Category.FirstOrDefault(c => c.Id == categoryId);
                if (category == null)
                {
                    return new ServiceResult<Category>
                    {
                        ResponseCode = ResponseCode.NOT_FOUND,
                    };
                }
                serviceResult.Result.Add(category);
            }
            catch
            {
                serviceResult.ResponseCode = ResponseCode.INTERNAL_SERVER_ERROR;
                serviceResult.ResponseMess = ResponseMessage.INTERNAL_SERVER_ERROR;
            }

            return serviceResult;
        }

        public async Task<ServiceResult<string>> Create(Category category)
        {
            ServiceResult<string> serviceResult = new ServiceResult<string>
            {
                ResponseCode = ResponseCode.SUCCESS,
                ResponseMess = CREATE_SUCCESS
            };
            try
            {
                //Kiểm tra xem tên chủ đề có bị trùng hay không
                validateCategory(category, ref serviceResult);
                //Không bị trùng tên chủ đề
                if (serviceResult.ResponseCode != ResponseCode.BAD_REQUEST)
                {
                    dbContext.Category.Add(category);
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

        public async Task<ServiceResult<string>> Update(Category category)
        {

            ServiceResult<string> serviceResult = new ServiceResult<string>
            {
                ResponseCode = ResponseCode.SUCCESS,
                ResponseMess = UPDATE_SUCCESS
            };
            try
            {
                //Kiểm tra xem tên chủ đề có bị trùng hay không
                validateCategory(category, ref serviceResult, isUpdate: true);
                //Không bị trùng tên chủ đề
                if (serviceResult.ResponseCode != ResponseCode.BAD_REQUEST)
                {
                    //Lấy ra category cũ trong db
                    Category categoryOld = dbContext.Category.FirstOrDefault(c => c.Id == category.Id);
                    //Sai id
                    if (categoryOld == null)
                    {
                        serviceResult.ResponseCode = ResponseCode.NOT_FOUND;
                        serviceResult.ResponseMess = NOT_FOUND;
                    }
                    //Cập nhật lại các thông tin
                    categoryOld.CategoryName = category.CategoryName;
                    dbContext.Category.Update(categoryOld);
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

        public async Task<ServiceResult<string>> Delete(Category category)
        {
            ServiceResult<string> serviceResult = new ServiceResult<string>
            {
                ResponseCode = ResponseCode.SUCCESS,
                ResponseMess = DELETE_SUCCESS
            };
            try
            {
                //Kiểm tra xem đã có bộ đề/câu hỏi nào thuộc chủ đề chưa
                if (dbContext.Exam.FirstOrDefault(e => e.CategoryId == category.Id) != null ||
                    dbContext.Question.FirstOrDefault(q => q.CategoryId == category.Id) != null)
                {
                    serviceResult.ResponseCode = ResponseCode.BAD_REQUEST;
                    serviceResult.ResponseMess = DELETE_FAILED;
                }
                else
                {
                    dbContext.Category.Remove(dbContext.Category.FirstOrDefault(c => c.Id == category.Id));
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

        //Kiểm tra xem tên chủ đề có bị trùng hay không
        private void validateCategory(Category category, ref ServiceResult<string> serviceResult, bool isUpdate = false)
        {
            //Thêm
            if (isUpdate == false)
            {
                if (dbContext.Category.FirstOrDefault(c => c.CategoryName.ToLower() == category.CategoryName.ToLower()) != null)
                {
                    serviceResult.ResponseCode = ResponseCode.BAD_REQUEST;
                    serviceResult.ResponseMess = EXISTS_CATEGORY_NAME;
                }
            }
            //Cập nhật
            else
            {
                if (dbContext.Category.FirstOrDefault(c => c.CategoryName.ToLower() == category.CategoryName.ToLower() && c.Id != category.Id) != null)
                {
                    serviceResult.ResponseCode = ResponseCode.BAD_REQUEST;
                    serviceResult.ResponseMess = EXISTS_CATEGORY_NAME;
                }
            }

        }
    }
}