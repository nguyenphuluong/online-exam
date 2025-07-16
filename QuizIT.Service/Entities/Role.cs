using System;
using System.Collections.Generic;

namespace QuizIT.Service.Entities
{
    public partial class Role
    {
        public Role()
        {
            User = new HashSet<User>();
        }

        public int Id { get; set; }
        public string RoleName { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}
