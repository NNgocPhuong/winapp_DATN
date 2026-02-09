using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WinApp.Views
{
    internal abstract class StationViewer<T> : AlarmViewer<T>
        where T : FrameworkElement, new()
    {
        public Station Station => ((IStation)DataContext).Station;
        abstract protected void OnSelected();
        protected override void ProcessAlarm(string id, AlarmMessage message)
        {
            if (id == Station.ObjectId)
                ProcessAlarm(message);
        }

        abstract protected void ProcessAlarm(AlarmMessage message);
        public StationViewer()
        {
            this.RegisterClickEvent(OnSelected);
        }
    }

    internal class HomeStationItemView : StationViewer<StationItemView>
    {
        protected override void OnSelected()
        {
            App.Request("/open", Station);
        }
        protected override void ProcessAlarm(AlarmMessage message)
        {
        }
    }

    internal class AlarmStationItemView : StationViewer<AlarmStation>
    {
        protected override void OnSelected()
        {
            App.Request("/open", Station);
        }
        protected override void ProcessAlarm(AlarmMessage message)
        {
        }
    }

    internal class DeviceToUserItemView : StationViewer<SelectStationItem>
    {
        protected override void OnSelected()
        {
        }

        protected override void ProcessAlarm(AlarmMessage message)
        {
        }
    }
}
