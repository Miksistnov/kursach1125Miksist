using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesDataAnalysisApp.Users
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Пароль в открытом виде
        public Role Role { get; set; }
        public bool IsBlocked { get; set; }
    }

}
