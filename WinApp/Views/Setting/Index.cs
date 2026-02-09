using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vst.Controls;

namespace WinApp.Views.Setting
{
    internal class Index : BaseView<MyTemplateForm>
    {
        protected override void RenderCore()
        {
            ViewModel = new EditContext("profile") { 
                Value = Global.User.Profile,
            };
        }
    }
}
