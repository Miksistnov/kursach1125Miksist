using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalesDataAnalysisApp.Users;
namespace SalesDataAnalysisApp.Model
{
    public static class AppState
    {
        public static User CurrentUser { get; set; }
    }
}
