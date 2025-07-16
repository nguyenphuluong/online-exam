using QuizIT.Common.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace QuizIT.Common.Models
{
    public class CurrentUser
    {
        public static int Id
        {
            get
            {
                var claims = HttpHelper.HttpContext.Items["CurrentUser"] as IEnumerable<Claim>;
                if (!int.TryParse(claims?.Where(x => x.Type == ClaimTypes.NameIdentifier)?.FirstOrDefault()?.Value, out int uId))
                {
                    uId = -1;   //Giá trị khi chưa đăng nhập là -1
                }
                return uId;
            }
            set { }
        }

        public static string Name
        {
            get
            {
                var claims = HttpHelper.HttpContext.Items["CurrentUser"] as IEnumerable<Claim>;
                if (claims == null)
                {
                    return "";
                }
                return claims?.Where(x => x.Type == ClaimTypes.Name)?.FirstOrDefault()?.Value;
            }
            set { }
        }

        public static int Role
        {
            get
            {
                var claims = HttpHelper.HttpContext.Items["CurrentUser"] as IEnumerable<Claim>;
                if (!int.TryParse(claims?.Where(x => x.Type == ClaimTypes.Role)?.FirstOrDefault()?.Value, out int uId))
                {
                    uId = -1;   //Giá trị khi chưa đăng nhập là -1
                }
                return uId;
            }
            set { }
        }
    }
}