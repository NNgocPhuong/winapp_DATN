using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace System.Windows.Controls
{
    public static class ControlExtension
    {
        static public void Render(this FrameworkElement element, Document context)
        {
            var type = element.GetType();
            foreach (var p in context)
            {
                var prop = type.GetProperty(p.Key);
                if (prop != null)
                {
                    try
                    {
                        prop.SetValue(element, p.Value);
                    }
                    catch
                    {

                    }
                }
            }

        }
        static public void RegisterClickEvent(this FrameworkElement element, Action clicked)
        {
            bool down = false;
            element.Cursor = Input.Cursors.Hand;
            element.MouseLeftButtonDown += (s, e) => down = true;
            element.MouseLeftButtonUp += (s, e) => {
                if (down)
                {
                    down = false;
                    clicked();
                }
            };
            element.MouseLeave += (s, e) => down = false;
        }
        static public double GetScreenWidth(this FrameworkElement element)
        {
            return SystemParameters.PrimaryScreenWidth;
        }
        static public double GetScreenHeight(this FrameworkElement element)
        {
            return SystemParameters.PrimaryScreenHeight;
        }

        static public void RefreshDataModel(this FrameworkElement element)
        {
            element.Dispatcher.InvokeAsync(() => {
                var model = element.DataContext;
                element.DataContext = null;
                element.DataContext = model;
            });
        }
        static public void DataModelChangedTo<T>(this FrameworkElement element, Action<T> visit)
        {
            element.DataContextChanged += (_, __) => {
                GetDataModel<T>(element, visit);
            };
        }
        static public bool GetDataModel<T>(this FrameworkElement element, Action<T> visit)
        {
            if (element.DataContext is T model)
            {
                visit(model);
                return true;
            }
            return false;
        }

    }

    public static class GridExtension
    {
        public static void AnimateGridLength(double from, double to, Action<GridLength> apply)
        {

            // Tạo animation từ width hiện tại sang width mới
            DoubleAnimation animation = new DoubleAnimation {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            // Khi animation chạy, cập nhật lại Width
            animation.Completed += (s, e) => {
                apply(new GridLength(animation.To.Value));
            };

            // Dùng AnimationClock để apply tạm thời
            var clock = animation.CreateClock();
            clock.CurrentTimeInvalidated += (s, e) =>
            {
                if (clock.CurrentProgress.HasValue)
                {
                    double current = animation.From.Value +
                                     (animation.To.Value - animation.From.Value) * clock.CurrentProgress.Value;
                    apply(new GridLength(current));
                }
            };

            clock.Controller.Begin();
        }

        public static void FlyOut(this ColumnDefinition column)
        {
            if (column.Width.Value > 0)
                return;

            var value = double.Parse(column.Tag.ToString());
            AnimateGridLength(0, value, g => column.Width = g);
        }
        public static void FlyIn(this ColumnDefinition column, Action before)
        {
            if (column.Width.Value == 0)
                return;

            if (before != null)
            {
                before();
            }

            AnimateGridLength(column.Width.Value, 0, g => column.Width = g);
        }
        public static void Show(this ColumnDefinition column, bool show = true)
        {
            column.Width = new GridLength(show ? double.Parse(column.Tag.ToString()) : 0);
        }

        public static void FlyOut(this RowDefinition row)
        {
            if (row.Height.Value > 0)
                return;

            var value = double.Parse(row.Tag.ToString());
            AnimateGridLength(0, value, g => row.Height = g);
        }
        public static void FlyIn(this RowDefinition row)
        {
            if (row.Height.Value == 0)
                return;

            AnimateGridLength(row.Height.Value, 0, g => row.Height = g);
        }
        public static void Show(this RowDefinition row, bool show = true)
        {
            row.Height = new GridLength(show ? double.Parse(row.Tag.ToString()) : 0);
        }

        public static FrameworkElement Add(this Grid grid, FrameworkElement element, int row, int column)
        {
            grid.Children.Add(element);
            element.SetValue(Grid.RowProperty, row);
            element.SetValue(Grid.ColumnProperty, column);

            return element;
        }
    }
}
