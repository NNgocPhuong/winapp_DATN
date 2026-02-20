using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Vst.Controls.SVG;

using VS = Vst.Controls.SVG;

namespace Vst.Controls
{
    public class Pen : LinkedList<Point>
    {
        double left, right;
        double top, bottom;

        public Pen Start(double x, double y)
        {
            left = right = x;
            top = bottom = y;

            this.Clear();
            this.AddLast(new Point(x, y));
            return this;
        }
        public Pen MoveTo(double x, double y)
        {
            if (x < left) left = x;
            if (x > right) right = x;
            if (y < top) top = y;
            if (y > bottom) bottom = y;

            this.AddLast(new Point(x, y));
            return this;
        }

        public Pen Move(double dx, double dy) => MoveTo(Last.Value.X, Last.Value.Y);
        public Pen VerticalMove(double dy) => Move(0, dy);
        public Pen HorizontalMove(double dx) => Move(dx, 0);

        public Pen Close() => MoveTo(A.X, A.Y);

        public Point A => First.Value;
        public Point B => Last.Value;

        public double Left => left;
        public double Right => right;
        public double Top => top;
        public double Bottom => bottom;
        public double Width => right - left;
        public double Height => bottom - top;

        public Shape Line()
        {
            if (this.Count > 2)
            {
                return new Polyline
                {
                    Points = new PointCollection(this),
                };
            }

            return new Line
            {
                X1 = A.X,
                Y1 = A.Y,
                X2 = B.X,
                Y2 = B.Y,
            };
        }
        public Shape Ellipse()
        {
            return new Ellipse
            {
                Width = Width,
                Height = Height,
                Margin = new Thickness(Left, Top, 0, 0)
            };
        }
    }
    public class SvgLayer : Canvas, VS.IAddChild
    {
        public void AppendChild(object child) => Children.Add((UIElement)child);
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            switch (e.Property.Name)
            {
                case nameof(Fill):
                case nameof(Stroke):
                case nameof(StrokeThickness):
                    SetAttribute(e.Property, e.NewValue);
                    return;
            }
            base.OnPropertyChanged(e);
        }

        public Brush Fill
        {
            get => (Brush)GetValue(Shape.FillProperty); 
            set => SetValue(Shape.FillProperty, value);
        }
        public Brush Stroke
        {
            get => (Brush)GetValue(Shape.StrokeProperty); 
            set => SetValue(Shape.StrokeProperty, value);
        }
        public double StrokeThickness
        {
            get => (double)GetValue(Shape.StrokeThicknessProperty);
            set => SetValue(Shape.StrokeThicknessProperty, value);
        }

        protected void SetAttribute(DependencyProperty property, object value)
        {
            foreach (Shape e in Children)
            {
                e.SetValue(property, value);
            }
        }
    }
    public class SvgCanvas : Viewbox, VS.IAddChild
    {
        Canvas _container;
        protected void SetCanvasSize(double width, double height)
        {
            _container.Width = width;
            _container.Height = height;
        }

        public SvgCanvas()
        {
            _container = new Canvas { };

            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;
            Stretch = Stretch.Uniform;

            Child = _container;
        }

        public void AppendChild(object child) => _container.Children.Add((UIElement)child);
        public void Clear() => _container.Children.Clear();
        protected void SetAttribute(DependencyProperty property, object value)
        {
            foreach (UIElement e in _container.Children)
            {
                if (e is Shape)
                {
                    e.SetValue(property, value);
                }
                else if (e is SvgCanvas)
                {
                    ((SvgCanvas)e).SetAttribute(property, value);
                }
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            switch (e.Property.Name)
            {
                case nameof(Fill):
                    _container.SetAttribute(Shape.FillProperty, e.NewValue);
                    return;

                case nameof(Stroke):
                    _container.SetAttribute(Shape.StrokeProperty, e.NewValue);
                    return;
            }
            base.OnPropertyChanged(e);
        }

        public Brush Fill { get => (Brush)GetValue(Shape.FillProperty); set => SetValue(Shape.FillProperty, value); }
        public Brush Stroke { get => (Brush)GetValue(Shape.StrokeProperty); set => SetValue(Shape.StrokeProperty, value); }
    }
    public class SvgIcon : SvgCanvas
    {
        static public VS.ElementCollection Resource { get; private set; } = new VS.ElementCollection();
        static public void Register(string path)
        {
            foreach (var name in System.IO.Directory.GetFiles(path))
            {
                var names = System.IO.Path.GetFileName(name).ToLower().Split('.');
                var k = names[0];

                switch (names[1])
                {
                    case "json":
                        var context = Document.Parse(System.IO.File.ReadAllText(name));
                        Resource.Add(k, new Element(context));
                        break;
                    case "svg":
                        var doc = new System.Xml.XmlDocument();
                        doc.Load(name);

                        Resource.Add(k, new Element(doc.DocumentElement));
                        break;

                }
            }
        }

        static readonly ShapeCreator _shapes = new ShapeCreator {
            { "svg", typeof(SvgCanvas) },
            { "g", typeof(SvgCanvas) },
            { "line", typeof(Line) },
            { "path", typeof(Path) },
            { "rect", typeof(Rectangle) },
            { "circle", typeof(Ellipse) },
            { "ellipse", typeof(Ellipse) },
        };

        static readonly AttributeConverter _attributes = new AttributeConverter {
            { typeof(ColorAttribute), v => v.Equals("none") ? null : new BrushConverter().ConvertFromString((string)v) },
            { typeof(PointsAttribute), GetPoints },
            { typeof(DataPathAttribute), GetPathData },
            { typeof(ViewBoxAttribute), GetViewBox },
        };

        static object GetViewBox(object value)
        {
            var v = (double[])value;
            return new Rect(0, 0, v[2], v[3]);
        }
        static object GetPathData(object value)
        {
            var s = ((string)value)
                .Replace("\r\n", " ")
                .Replace(',', '.'); // normalize decimals to invariant form

            return Geometry.Parse(s);
        }
        static object GetPoints(object value)
        {
            var pts = new PointCollection();
            var v = (double[])value;

            int i = 0;
            while (i < v.Length)
            {
                var x = v[i++];
                var y = v[i++];
                pts.Add(new Point(x, y));
            }
            return pts;
        }
        public SvgIcon()
        {
            Width = Height = 24;
        }

        static public readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached(
                nameof(Source), typeof(string), typeof(SvgIcon),
                new PropertyMetadata(string.Empty, OnSourceChanged));

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var k = ((string)e.NewValue).ToLower();
            if (Resource.TryGetValue(k, out var element))
            {
                ((SvgIcon)d).LoadElement(element);
            }
        }
        protected virtual void LoadElement(VS.Element element)
        {
            try
            {
                Clear();
                element.Attributes.GetAttribute<ViewBoxAttribute>("viewBox", a =>
                {
                    var c = (Canvas)Child;
                    c.Width = a.Width;
                    c.Height = a.Height;
                });
                Dispatcher.InvokeAsync(() =>
                {
                    element.Render(_shapes, _attributes, this);
                    SetAttribute(Shape.FillProperty, Fill);
                    SetAttribute(Shape.StrokeProperty, Stroke);
                });
            }
            catch
            {
            }
        }
    }
}
