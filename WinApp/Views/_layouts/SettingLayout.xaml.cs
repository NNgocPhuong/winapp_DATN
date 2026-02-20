using System.Windows.Controls;
using Vst.Controls;

namespace WinApp.Views
{
    public partial class SettingLayout : UserControl
    {
        public SettingLayout()
        {
            InitializeComponent();

            BtnLogout.RegisterClickEvent(() =>
            {
                this.Request("/logout");
            });
        }
    }
}