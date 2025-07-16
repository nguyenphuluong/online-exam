using System;
using System.Collections.Generic;

namespace QuizIT.Service.Entities
{
    public partial class History
    {
        public History()
        {
            HistoryDetail = new HashSet<HistoryDetail>();
        }

        public int Id { get; set; }
        public int ExamId { get; set; }
        public int UserId { get; set; }
        public double TimeDoExam { get; set; }
        public int Point { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Exam Exam { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<HistoryDetail> HistoryDetail { get; set; }
    }
}
