using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Controllers
{
    internal class HomeController : BaseController
    {
        public override object Index()
        {
            if (Global.Stations == null)
            {
                return View(new Views.Home.Loading(), "home");
            }
            return base.Index();
        }
        public object Open(Models.Station model) 
        {
            Global.Stations.Current = model;
            if (model.IsReady == false)
            {
                return View(new Views.Home.Loading(), "/monitoring");
            }
            return View(new Views.Home.Monitoring(), model);
        }
        public object Monitoring() => View();
    }
}
