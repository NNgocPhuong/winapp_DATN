using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Vst.Controls
{
    public interface IListView
    {
        UIElementCollection Children { get; }
        System.Collections.IEnumerable ItemsSource { get; set; }
        event Action<object> ItemClick;
    }

}
