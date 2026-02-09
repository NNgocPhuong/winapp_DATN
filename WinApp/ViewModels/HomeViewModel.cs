using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Models;
using Services;
namespace WinApp
{
    internal class HomeViewModel : AsyncViewModel
    {
        public List<IStation> Stations
        {
            get
            {
                var list = new List<IStation>();
                foreach (Station e in Global.Stations.Values)
                {
                    list.Add(new StationViewModel { Station = e });
                }
                return list;
            }
        }
        
        public List<IStation> Alarms
        {
            get
            {
                var list = new List<IStation>();
                foreach (Station s in Global.Stations.Values)
                {
                    var a = s.AlarmMessage;
                    if (a != null)
                    {
                        list.Add(new AlarmViewModel(a));
                    }
                }
                return list;
            }
        }

        public HomeViewModel()
        {
        }

        public override bool IsReady => Global.Stations != null;
        public override void Execute()
        {
        }

        public override object GetDataContext()
        {
            return this;
        }
    }
}
