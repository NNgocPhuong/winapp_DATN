using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp
{
    class LoginContext : EditContext
    {
        public LoginContext() : base("login")
        {
            Value = new Document {
                UserName = "0902186628",
                Password = "6628",
            };
        }
    }
}
