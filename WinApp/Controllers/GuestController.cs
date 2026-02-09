using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Controllers
{
    class GuestController : BaseController
    {
        public object Login(EditContext context)
        {
            return Send("manager", "account/login", context.ValueContext);

            //App.User = new Actors.Admin();
            //return Redirect(App.User.GetType().Name);
        }
    }
}
