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
    /// Interaction logic for MainLayout.xaml
    /// </summary>
    public partial class MainLayout : UserControl
    {
        public MainLayout()
        {
            InitializeComponent();

            searchBox.Render(new ViewModels.SearchContext(null, s => MessageBox.Show(s), null, null));
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
