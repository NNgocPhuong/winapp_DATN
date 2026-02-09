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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vst.Controls;

namespace WinApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Views.MainLayout mainLayout;
        public void UpdateView(object content)
        {
            Dispatcher.InvokeAsync(() => {
                if (mainLayout == null)
                {
                    Content = mainLayout = new Views.MainLayout { };
                    this.WindowStyle = WindowStyle.ThreeDBorderWindow;
                }
                if (content == null)
                {
                    BeginWaiting();
                    return;
                }
                if (content is FrameworkElement element)
                {
                    Task.Run(async () => {
                        await Dispatcher.InvokeAsync(() => {
                            mainLayout.mainContent.Child = element;
                            mainLayout.SetActiveTabbedBarItem(System.Mvc.Engine.RequestContext.ControllerName);
                        });
                        EndWaiting();
                    });


                }
            });


        }
        public void Start()
        {
            App.Current.MainWindow = this;

            App.Request("home");
            Show();
        }

        internal void BeginWaiting()
        {
            Dispatcher.InvokeAsync(() => mainLayout.Fade.Visibility = Visibility.Visible);
        }
        internal void EndWaiting()
        {
            Dispatcher.InvokeAsync(() => mainLayout.Fade.Visibility = Visibility.Collapsed);
        }

        public MainWindow()
        {
            this.InitializeComponent();
            TabbarItem.ActiveColor = (SolidColorBrush)App.Current.Resources["PrimaryBrush"];

            UpdateView(null);
            Global.AlarmReceived += (id, context) => {
                this.WindowState = WindowState.Maximized;
            };
        }
    }
}
