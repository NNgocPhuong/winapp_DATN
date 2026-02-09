using Services;
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
    /// Interaction logic for PhoneItemView.xaml
    /// </summary>
    public partial class PhoneItemView : UserControl
    {
        public PhoneItemView()
        {
            InitializeComponent();

            PhoneNumberService model = null;

            Action<Border, int> updateButton = (btn, v) => { 
                btn.Background = v == 0 ? Brushes.WhiteSmoke : Brushes.DarkGreen;
                ((SvgIcon)btn.Child).Fill = v == 0 ? Brushes.Gray : Brushes.White;
            };
            Action update = () => {

                if (model.Action == "-")
                {
                    ((Panel)this.Parent)?.Children?.Remove(this);
                }    

                updateButton(BtnSms, model.Sms);
                updateButton(BtnCall, model.Call);
            };

            this.BtnDelete.Click += (_, __) => model.BeginDelete();

            BtnSms.RegisterClickEvent(() => model.Switch("sms"));
            BtnCall.RegisterClickEvent(() => model.Switch("call"));

            this.DataModelChangedTo<PhoneNumberService>(vm => {
                model = vm;
                update();

                vm.Completed += () => Dispatcher.InvokeAsync(update);
            });
        }
    }
}
