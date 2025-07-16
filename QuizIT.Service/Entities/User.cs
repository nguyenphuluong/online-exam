using System;
using System.Collections.Generic;

namespace QuizIT.Service.Entities
{
    public partial class User
    {
        public User()
        {
            Exam = new HashSet<Exam>();
            History = new HashSet<History>();
            Question = new HashSet<Question>();
            Rank = new HashSet<Rank>();
        }

        public int Id { get; set; }
        public int RoleId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime CreateAt { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<Exam> Exam { get; set; }
        public virtual ICollection<History> History { get; set; }
        public virtual ICollection<Question> Question { get; set; }
        public virtual ICollection<Rank> Rank { get; set; }
    }
}
