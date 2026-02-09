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
    using Services;
    using ViewModels;
    using Vst.Controls;

    /// <summary>
    /// Interaction logic for CalendarView.xaml
    /// </summary>
    public partial class CalendarView : UserControl
    {
        static string[] daysOfWeek = new string[] {
            "SUN",
            "MON",
            "TUE",
            "WED",
            "THU",
            "FRI",
            "SAT",
        };

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            double width = sizeInfo.NewSize.Width;
            if (width > 0)
            {
                var w = width / 7;
                foreach (var cd in daysContent.ColumnDefinitions)
                {
                    cd.Width = new GridLength(w);
                }
                var h = daysContent.RowDefinitions[0].Height.Value;
                
                w = 30;
                for (int r = 0; r < 6; r++, h += w)
                    daysContent.RowDefinitions[r + 1].Height = new GridLength(w);
                
                mainContent.RowDefinitions[1].Height = new GridLength(h);
            }
            base.OnRenderSizeChanged(sizeInfo);
        }

        public CalendarView()
        {
            InitializeComponent();

            for (int i = 0; i < 7; i++)
            {
                var label = new Label { 
                    Content = daysOfWeek[i],
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold,
                };
                daysContent.ColumnDefinitions.Add(new ColumnDefinition());

                if (i == 0)
                {
                    label.Foreground = Brushes.Red;
                }
                daysContent.Add(label, 0, i);
            }
            for (int r = 0; r < 6; r++) {
                daysContent.RowDefinitions.Add(new RowDefinition());
            }

            this.DataModelChangedTo<MonthHistoryService>(vm => {
                foreach (var k in vm.Alarms.Keys)
                {
                    var cell = (GridLayout)_cells[int.Parse(k) - 1];
                    cell.BorderBrush = Brushes.Black;
                    cell.Cursor = Cursors.Hand;

                    cell.MouseMove += (_, __) => cell.Background = Brushes.Orange;
                    cell.MouseLeave += (_, __) => cell.Background = Brushes.White;

                    cell.RegisterClickEvent(() => OneDaySelected?.Invoke(vm.GetDayAlarm(k)));
                }
            });
        }

        public event Action<DocumentList> OneDaySelected;
        FrameworkElement[] _cells;
        
        public void CreateContent(string month)
        {
            if (_cells != null)
            {
                foreach (var cell in _cells)
                {
                    daysContent.Children.Remove(cell);
                }
            }

            var first = DateTime.Parse(month + "-01");
            int r = 1, c = (int)first.DayOfWeek;
            int days = (int)(first.AddMonths(1) - first).TotalDays;
            int d = 0;

            header.Content = $"Tháng {first.Month}, {first.Year}";

            _cells = new FrameworkElement[days];
            while (d < days)
            {
                var label = new TextBlock { 
                    Text = $"{d + 1}",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                
                var cell = new GridLayout { 
                    Children = { label },
                    Margin = new Thickness(1),
                    Background = Brushes.White,
                    BorderThickness = new Thickness(0.5),
                    BorderBrush = Brushes.LightGray,
                };
                daysContent.Add(_cells[d++] = cell, r, c);

                if (++c == 7) { c = 0; r++; }
            }
        }
    }
}
