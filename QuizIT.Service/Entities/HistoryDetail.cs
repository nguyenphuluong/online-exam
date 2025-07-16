using System;
using System.Collections.Generic;

namespace QuizIT.Service.Entities
{
    public partial class HistoryDetail
    {
        public int Id { get; set; }
        public int HistoryId { get; set; }
        public int QuestionId { get; set; }
        public string AnswerSelect { get; set; }

        public virtual History History { get; set; }
        public virtual Question Question { get; set; }
    }
}
