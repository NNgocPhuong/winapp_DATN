using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
    public class FireIcon : SvgIcon
    {
        public FireIcon() { Source = "fire"; Fill = Brushes.Red; }
    }
    public class WarningIcon : SvgIcon
    {
        public WarningIcon() { Source = "warning"; Fill = Brushes.Orange; }
    }
    public class BoltIcon : SvgIcon
    {
        static Brush bolt_color = Brushes.DarkMagenta;
        public BoltIcon() { Source = "bolt"; Fill = bolt_color; }
    }

    public enum AlarmIconOrientation
    {
        None,
        Horizontal,
        Vertical,
    };
    public partial class AlarmIcon : GridLayout
    {
        static public MyElement CreateIcon(int mode)
        {
            switch (mode)
            {
                case 1: return new MyBorder<WarningIcon>();
                case 2: return new MyBorder<BoltIcon>();
            }
            return new MyBorder<FireIcon>();
        }

        public AlarmIcon()
        {
            for (int i = 0; i < 3; i++)
            {
                Children.Add(CreateIcon(i + 1));
            }
            IconOpacity = 0;
        }

        public void ForEach(Action<MyElement> action)
        {
            foreach (MyElement al in this.Children) 
                action(al);
        }

        double _iconSize;
        public double IconSize
        {
            get => _iconSize;
            set
            {
                _iconSize = value;
                ForEach(a => a.Width = value);
            }
        }

        double _iconOpactity = 0;
        public double IconOpacity
        {
            get => _iconOpactity;
            set
            {
                _iconOpactity = value;
                ForEach(a => a.Opacity = value);
            }
        }

        public AlarmIconOrientation Orientation
        {
            get => this.Columns.Count == 1 ? AlarmIconOrientation.Vertical 
                : (this.Rows.Count == 1 ? AlarmIconOrientation.Horizontal 
                : AlarmIconOrientation.None);
            set
            {
                this.Columns.Clear();
                this.Rows.Clear();
                if (value == AlarmIconOrientation.Vertical)
                {
                    this.Split(this.Children.Count, 1);
                }
                else if (value == AlarmIconOrientation.Horizontal)
                {
                    this.Split(1, this.Children.Count);
                }
            }
        }
        public MyElement Select(int index, Action<MyElement> callback)
        {
            MyElement indicator = null;
            Dispatcher.InvokeAsync(() => {
                ForEach(a => {
                    a.StopBlink(_iconOpactity);
                });

                if (index != 0)
                {
                    indicator = (MyElement)this.Children[index - 1];
                    callback?.Invoke(indicator);
                }
            });
            return indicator;
        }
        public void Reset()
        {
            this.StopBlink(1);
            ForEach(a => a.StopBlink(_iconOpactity));
        }
    }
}
