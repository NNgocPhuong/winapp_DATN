using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using OxyPlot;
using OxyPlot.Series;

namespace WinApp.Views
{
    class StationNowChartView : MyChartView
    {
        public StationNowChartView()
        {
        }
        public void Update(ChartData data)
        {
            if (data.FirstRecord == null || data.FirstRecord.Time == null)
                return;

            var area = new AreaSeries
            {
                Color = OxyColor.FromRgb(0x62, 0x4c, 0xfd),
                Color2 = OxyColor.FromArgb(0, 0, 0, 0),
                StrokeThickness = 2,
                MarkerType = MarkerType.None,
            };

            var y = data.GetData();
            for (int x = 0; x < y.Length; x++)
            {
                area.Points.Add(new DataPoint(x, (double)y[x].Value));
                area.Points2.Add(new DataPoint(x, 0));
            }

            var model = new MyPlotModel();
            model.Oy.Maximum = data.Max;

            model.Ox.Maximum = Math.Max(60, y.Length);
            model.Ox.Minimum = model.Ox.Maximum - 60;

            ((MyTimeAxis)model.Ox).Start = data.FirstRecord.Time.Value;

            model.Series.Add(area);
            Model = model;
        }
    }
}
