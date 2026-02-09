using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WinApp.Views
{
    /// <summary>
    /// Interaction logic for MonitoringLayout.xaml
    /// </summary>
    public partial class MonitoringLayout : UserControl
    {
        MonitoringViewModel vm;

        //public void UpdateAlarm(string id, AlarmMessage message)
        //{
        //    if (id == vm.Station.ObjectId)
        //    {
        //        Dispatcher.InvokeAsync(() => {
        //            LastAlarmList.Add(message);
        //            this.StationView.AlamIcons.Select(message.Code, a => a.Blink(0, 1));
        //        });
        //    }
        //}
        public MonitoringLayout()
        {
            InitializeComponent();

            this.Visibility = Visibility.Collapsed;
            this.DataModelChangedTo<MonitoringViewModel>(vm => {
                this.vm = vm;
                vm.UpdateView += () => {
                    Dispatcher.InvokeAsync(() => {
                        StationView.Update();
                        SummaryList.ItemsSource = vm.Station.AlarmSummary;
                        LastAlarmList.ItemsSource = vm.Station.LastAlarm;

                        this.Visibility = Visibility.Visible;
                    });
                };
            });

            SummaryList.ItemClick += e => {

                flyingColumn.Show();

                var date = ((Document)e).ObjectId;
                Calendar.CreateContent(date);

                vm.GetMonth(date, monthHistoryService => {

                    Dispatcher.InvokeAsync(() => {
                        Calendar.DataContext = monthHistoryService;
                    });
                });
            };

            Calendar.OneDaySelected += list => DayAlarmList.ItemsSource = list;
            ButtonCloseCalendar.Click += (s, e) => flyingColumn.Show(false);
        }
    }
}
