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
    /// Interaction logic for HomeLayout.xaml
    /// </summary>
    public partial class HomeLayout : UserControl
    {
        public HomeLayout()
        {
            InitializeComponent();
            this.DataModelChangedTo<HomeViewModel>(vm => {
                //if (alarms.Count > 0)
                //{
                //    FlyoutColumn.Show();
                //    alarmsContent.ItemsSource = alarms;
                //}
            });
        }
    }

    internal class HomeStationList : AlarmViewer<ScrollViewer>
    {
        static Thickness itemMargin = new Thickness(2.5);
        class ListView : InlineBlockView {
            protected override FrameworkElement CreateItem() => new HomeStationItemView { 
                Margin = itemMargin,
            };
        };

        ListView listView;
        protected override void ProcessAlarm(string id, AlarmMessage message)
        {
        }
        public HomeStationList()
        {
            listView = new ListView { 
                Padding = new Thickness(10)
            };
            MainContent.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            MainContent.Content = listView;

            this.DataModelChangedTo<HomeViewModel>(vm => {
                var stations = vm.Stations;
                var alarms = vm.Alarms;

                listView.ItemsSource = stations;

                //foreach (IStation s in stations)
                //{
                //    //s.ShowAlarm = () => {
                //    //    Dispatcher.InvokeAsync(() =>
                //    //    {
                //    //        FlyoutColumn.FlyOut();
                //    //        alarmsContent.ItemsSource = vm.Alarms;
                //    //    });
                //    //};
                //}
            });

        }
    }
    
    internal class AlarmStationList : AlarmViewer<ScrollViewer>
    {
        class ListView : MyListView
        {
            protected override FrameworkElement CreateItem() => new AlarmStationItemView();
        };

        protected override void ProcessAlarm(string id, AlarmMessage message)
        {
            listView.ItemsSource = ((HomeViewModel)DataContext).Alarms;
        }

        ListView listView;
        public AlarmStationList()
        {
            listView = new ListView {
                Padding = new Thickness(10)
            };

            MainContent.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            MainContent.Content = listView;
            
            this.DataModelChangedTo<HomeViewModel>(vm => {
                var alarms = vm.Alarms;

                listView.ItemsSource = alarms;

                //foreach (IStation s in stations)
                //{
                //    //s.ShowAlarm = () => {
                //    //    Dispatcher.InvokeAsync(() =>
                //    //    {
                //    //        FlyoutColumn.FlyOut();
                //    //        alarmsContent.ItemsSource = vm.Alarms;
                //    //    });
                //    //};
                //}
            });
        }
    }
}
