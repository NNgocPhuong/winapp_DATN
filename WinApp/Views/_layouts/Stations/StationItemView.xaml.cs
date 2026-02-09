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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WinApp.Views
{
    /// <summary>
    /// Interaction logic for StationItemView.xaml
    /// </summary>
    public partial class StationItemView : UserControl
    {
        public StationItemView()
        {
            InitializeComponent();

            this.DataModelChangedTo<Models.Station>(vm => {
                //vm.AlarmReceived += msg => {
                //    this.Dispatcher.InvokeAsync(() => {
                //        AlarmIcons.Select(msg.Code, a => a.Blink(0.25, 1));
                //    });
                //};
            });
        }
    }
}
