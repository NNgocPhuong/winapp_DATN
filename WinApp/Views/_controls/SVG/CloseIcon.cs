using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Vst.Controls
{

    public class CloseButon : ClickElement
    {
        CloseIcon _icon = new CloseIcon { StrokeThickness = 2 };
        public CloseButon() 
        {
            Child = _icon;
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Right;

            Opacity = 0.5;
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            Opacity = 0.5;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            Opacity = 1;
        }
        protected override void OnRender(DrawingContext dc)
        {
            _icon.Stroke = Foreground;
            base.OnRender(dc);
        }
    }

    public class CloseIcon : SvgLayer
    {
        public CloseIcon() 
        {
            Width = Height = 16;

            var p = new Pen();
            p.Start(0, 0).MoveTo(Width, Height);

            AppendChild(p.Start(0, 0).MoveTo(Width, Height).Line());
            AppendChild(p.Start(0, Height).MoveTo(Width, 0).Line());

            Stroke = Brushes.Black;
        }
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == nameof(Fill))
            {
                SetAttribute(Shape.StrokeProperty, e.NewValue);
            }
            base.OnPropertyChanged(e);
        }
    }
}
