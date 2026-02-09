using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Vst.Controls;
namespace WinApp.Views
{
    internal class HistoryListView : MyListView
    {
        public HistoryListView()
        {
            this.Padding = new Thickness(10);
        }

        protected override FrameworkElement CreateItem()
        {
            return new DangerItemView();
        }
        protected override void AddItem(UIElement item)
        {
            this.Children.Insert(0, item);
        }
        public void Add(Models.AlarmMessage message)
        {
            AddItem(CreateItem(message));
        }

    }

    internal class MonthHistoryListView : HistoryListView
    {
        protected override FrameworkElement CreateItem() => new MonthHistoryItemView();
    }
}
