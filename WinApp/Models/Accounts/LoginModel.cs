using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    internal class LoginModel : BaseModel
    {
        public LoginModel() 
        {
            var doc = DB.Accounts.Find("#last");
            if (doc != null)
            {                
                this.UserName = doc.UserName;
                Token = doc.Token;
            }
        }
    }

}
