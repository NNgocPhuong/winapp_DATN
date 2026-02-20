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
using Services;
using Vst.Controls;

namespace WinApp.Views
{
    /// <summary>
    /// Interaction logic for MainLayout.xaml
    /// </summary>
    public partial class MainLayout : UserControl
    {
        public MainLayout()
        {
            InitializeComponent();

            searchBox.Render(new ViewModels.SearchContext(null, s => MessageBox.Show(s), null, null));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LogoutButton.IsEnabled = false;

            var service = new LogoutService();

            service.ResponseError += (code, message) =>
            {
                LogoutButton.Dispatcher.InvokeAsync(() =>
                {
                    LogoutButton.IsEnabled = true;
                    App.ShowError(message ?? "Logout failed.");
                });
            };

            service.ResponseSuccess += _ =>
            {
                LogoutButton.Dispatcher.InvokeAsync(() =>
                {
                    App.Current.Shutdown();
                });
            };

            service.Execute(new Document());
        }

        public void SetActiveTabbedBarItem(string name)
        {
            var k = name.ToLower();
            foreach (TabbarItem item in TabbedBar.Children)
            {
                var url = item.Url.ToLower();
                if (url == k)
                {
                    item.Activated = true;
                    return;
                }
            }
        }
    }
}
