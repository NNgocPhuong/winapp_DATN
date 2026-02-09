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

namespace WinApp.Views
{
    /// <summary>
    /// Interaction logic for DangerItemView.xaml
    /// </summary>
    public partial class DangerItemView : UserControl
    {
        public DangerItemView()
        {
            InitializeComponent();
            this.DataModelChangedTo<Models.AlarmMessage>(msg => {
                Dispatcher.InvokeAsync(() => Icon.Child = AlarmIcon.CreateIcon(msg.Code));
            });
        }
    }
}
