using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WinApp;

namespace Vst.Controls
{

    public class BaseListView<TContent> : MyPanel<TContent>, IListView
        where TContent : Panel, new()
    {
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(
                nameof(ItemTemplate),
                typeof(DataTemplate),
                typeof(BaseListView<TContent>),
                new PropertyMetadata(null));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(BaseListView<TContent>),
                new PropertyMetadata(null, OnItemsSourceChanged));

        protected virtual FrameworkElement CreateItem()
        {
            return (FrameworkElement)ItemTemplate.LoadContent();
        }
        protected FrameworkElement CreateItem(object dataContext)
        {
            var i = CreateItem();
            i.DataContext = dataContext;

            i.RegisterClickEvent(() => {
                ItemClick?.Invoke(dataContext);
            });
            return i;
        }
        protected virtual void AddItem(UIElement item)
        {
            Children.Add(item);
        }
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (BaseListView<TContent>)d;
            IEnumerable newValue = (IEnumerable)e.NewValue;

            if (newValue != null)
            {
                control.Children.Clear();
                foreach (var one in newValue)
                {
                    control.AddItem(control.CreateItem(one));
                }
            }
        }

        public event Action<object> ItemClick;
        public FrameworkElement Find(object context)
        {
            foreach (FrameworkElement control in Children) {
                if (control.DataContext == context)
                    return control;
            }
            return null;
        }

        public BaseListView()
        {
            this.DataModelChangedTo<ListContext>(context => {
                ItemClick += e => context.SelectedItem = e;
            });
        }
    }

    public class InlineBlockView : BaseListView<WrapPanel>
    {
        public string Url { get; set; } = "/open";
    }

    public class MyListView : BaseListView<StackPanel>
    {

    }
}
