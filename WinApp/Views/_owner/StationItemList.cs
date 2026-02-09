using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WinApp.Views
{
    internal class StationItemList : Vst.Controls.MyListView
    {
        protected override FrameworkElement CreateItem()
        {
            return new StationItemView();
        }

        public StationItemList()
        {
        }
    }
}
