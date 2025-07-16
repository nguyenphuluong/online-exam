using System;
using System.Collections.Generic;

namespace QuizIT.Service.Entities
{
    public partial class Exam
    {
        public Exam()
        {
            ExamDetail = new HashSet<ExamDetail>();
            History = new HashSet<History>();
            Rank = new HashSet<Rank>();
        }

        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string ExamName { get; set; }
        public int Time { get; set; }
        public DateTime CreateAt { get; set; }
        public int CreateBy { get; set; }
        public bool IsActive { get; set; }

        public virtual Category Category { get; set; }
        public virtual User CreateByNavigation { get; set; }
        public virtual ICollection<ExamDetail> ExamDetail { get; set; }
        public virtual ICollection<History> History { get; set; }
        public virtual ICollection<Rank> Rank { get; set; }
    }
}
