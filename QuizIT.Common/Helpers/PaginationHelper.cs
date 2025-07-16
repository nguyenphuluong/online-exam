using QuizIT.Common.Models;
using System;
using System.Collections.Generic;

namespace QuizIT.Common.Helpers
{
    public class PaginationHelper
    {
        public static PaginationModel GetPaginationModel(int pageNumber, int pageSize, int totalRecord, int maxPageDisplay = 5)
        {
            int startPage = 0, endPage = 0, middlePage = 0, totalPage = (int)Math.Ceiling(totalRecord * 1.0 / pageSize);
            var paginationModel = new PaginationModel();

            #region Xử lí page number nằm ngoài khoảng

            if (pageNumber < 1 || pageNumber > totalPage || totalPage == 0)
            {
                return new PaginationModel
                {
                    TotalPage = 0,
                    PageNumber = 0,
                    Pages = new List<int>(),
                    PageSize = pageSize,
                    MaxPageDisplay = maxPageDisplay
                };
            }

            #endregion Xử lí page number nằm ngoài khoảng

            #region Tính toán middle page, start page, end page

            middlePage = (int)Math.Ceiling(maxPageDisplay * 1.0 / 2);
            if (totalPage < maxPageDisplay)
            {
                startPage = 1;
                endPage = totalPage;
            }
            else
            {
                startPage = pageNumber - middlePage + 1;
                endPage = pageNumber + middlePage - 1;
                if (startPage < 1)
                {
                    startPage = 1;
                    endPage = maxPageDisplay;
                }
                else if (endPage > totalPage)
                {
                    startPage = totalPage - maxPageDisplay + 1;
                    endPage = totalPage;
                }
            }

            #endregion Tính toán middle page, start page, end page

            #region Nạp dữ liệu vào paginationModel

            paginationModel.PageNumber = pageNumber;
            paginationModel.PageSize = pageSize;
            paginationModel.TotalPage = totalPage;
            paginationModel.MaxPageDisplay = maxPageDisplay;
            for (int i = startPage; i <= endPage; i++)
            {
                paginationModel.Pages.Add(i);
            }

            #endregion Nạp dữ liệu vào paginationModel

            return paginationModel;
        }
    }
}