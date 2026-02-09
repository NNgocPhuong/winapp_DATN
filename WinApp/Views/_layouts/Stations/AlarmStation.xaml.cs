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

namespace WinApp.Views
{
    /// <summary>
    /// Interaction logic for AlarmStation.xaml
    /// </summary>
    public partial class AlarmStation : UserControl
    {
        public AlarmStation()
        {
            InitializeComponent();
            this.DataModelChangedTo<AlarmViewModel>(vm => {
                var icon = AlarmIcon.CreateIcon(vm.Code);
                icon.Blink(0.2, 1);

                Indicator.Child = icon;
            });
        }
    }
}
