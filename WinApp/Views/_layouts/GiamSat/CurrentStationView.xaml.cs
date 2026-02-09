using Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

using Vst.Controls;
namespace WinApp.Views
{
    /// <summary>
    /// Interaction logic for CurrentStationView.xaml
    /// </summary>
    public partial class CurrentStationView : UserControl
    {
        MonitoringViewModel vm;
        public CurrentStationView()
        {
            InitializeComponent();

            Action<GridLayout> create_property_table = g => { 
                int n = g.Children.Count;
                g.Split(n >> 1, 2);

                for (int i = 0; i < n; i += 2)
                {
                    var c = (EditorLabel)g.Children[i];
                    var v = (TextBlock)g.Children[i + 1];

                    v.HorizontalAlignment = HorizontalAlignment.Right;
                    v.FontSize = c.FontSize;
                }
            };

            Relay0.Click += (_, __) => {
                vm.Remote();
                Relay0.Blink(0.2, 0.8);
            };
            Relay0.DataModelChangedTo<RelayViewModel>(relay => {  
                Relay0.StopBlink(1);
                Relay0.Background = relay.Color;
                Relay0.Text = relay.Text;
            });

            this.DataModelChangedTo<MonitoringViewModel>(vm => {
                this.vm = vm;
                var gui = vm.Station.GetGUIConfig();

                StatusContent.Children.Clear();
                foreach (var k in gui.Keys)
                {
                    var monitor = new MonitoringBlock();
                    monitor.Render(gui, k);

                    StatusContent.Children.Add(monitor);
                }

                var alarm = vm.Station.AlarmMessage;
                if (alarm != null)
                {
                    SetAlarm(alarm);
                }

                //vm.StatusChanged += (r) => {
                //    Dispatcher.InvokeAsync(() => {
                //        ChartView.Update(vm);
                //        this.StatusRecord.DataContext = r;
                //    });
                //};
                // vm.AlarmReceived += (msg) => AlamIcons.Select(msg.Code, a => a.Blink(0.5, 1, 0.3));
            });
        }

        public void SetAlarm(AlarmMessage alarm)
        {
            AlamIcons.Select(alarm.Code, a => a.Blink(0.2, 1));
            vm.Station.ClearAlarm();
        }

        public void Update()
        {
            ChartView.Update(vm.ChartData);
            Relay0.DataContext = vm.Relay;
            StatusContent.DataContext = vm.ChartData.LastRecord;
        }
    }
}
