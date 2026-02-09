using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Views.Home
{
    internal class Loading : Views.Loading
    {
        public override void Render(object model)
        {
            var url = (string)model;
            switch (url)
            {
                case "home":
                    var service = new AccountService();
                    service.LoadStations(() => App.DispatcherRequest(url));
                    break;

                default:
                    var station = Global.Stations.Current;
                    station.LoadHistory();
                    while (!station.IsReady)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    App.DispatcherRequest(url);
                    break;
            }
        }
    }
}
