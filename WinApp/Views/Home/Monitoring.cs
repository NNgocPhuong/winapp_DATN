using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Views.Home
{
    internal class MonitoringViewer : AlarmViewer<MonitoringLayout>
    {
        protected override void ProcessAlarm(string id, AlarmMessage message)
        {
            var vm = (MonitoringViewModel)DataContext;
            if (id != vm.Station.ObjectId)
            {
                App.Request("home");
                return;
            }

            MainContent.StationView.SetAlarm(message);
            MainContent.LastAlarmList.Add(message);
        }
    }
    internal class Monitoring : BaseView<MonitoringViewModel, MonitoringViewer>
    {
        public override void Render(object model)
        {
            base.Render(model);
        }
    }
}
