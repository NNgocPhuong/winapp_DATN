using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WinApp.Views.Home
{
    internal class HomeViewer : AlarmViewer<HomeLayout>
    {
        protected override void ProcessAlarm(string id, AlarmMessage message)
        {
            MainContent.FlyoutColumn.FlyOut();
        }
    }
    internal class Index : BaseView<HomeViewModel, HomeViewer>
    {
    }
}
