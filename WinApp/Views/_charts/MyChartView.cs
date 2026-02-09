using System;
using System.Collections.Generic;
using System.Text;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;


namespace WinApp.Views
{
    
    class MyTimeAxis : LinearAxis
    {
        public DateTime Start { get; set; }
        public MyTimeAxis()
        {
            Position = AxisPosition.Bottom;
            MajorGridlineStyle = LineStyle.Solid;
            MinorGridlineStyle = LineStyle.Dot;
        }
        protected override string FormatValueOverride(double x)
        {
            var t = (int)x % 60;
            return (t == 0 ? "0" : $"{t}s");
        }
    }
    class MyCurrentAxis : LinearAxis
    {
        public MyCurrentAxis()
        {
            Position = AxisPosition.Left;
            MajorGridlineStyle = LineStyle.Solid;
            MinorGridlineStyle = LineStyle.Dot;
        }
        protected override string FormatValueOverride(double x)
        {
            if (x > 0.1)
            {
                return $"{x}A";
            }
            return base.FormatValueOverride(x);
        }
    }
    class MyPlotModel : PlotModel
    {
        public Axis Ox => Axes[0];
        public Axis Oy => Axes[1];
        public MyPlotModel()
        {
            Axes.Add(new MyTimeAxis());
            Axes.Add(new MyCurrentAxis());
        }
    }
    abstract class MyChartView : OxyPlot.Wpf.PlotView
    {
        public MyChartView()
        {
        }
    }
}
