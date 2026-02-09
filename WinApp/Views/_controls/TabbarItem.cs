using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Vst.Controls
{
    public class TabbarItem : MyPanel<StackPanel>
    {
        static public Brush ActiveColor { get; set; }
        static public Brush NormalColor { get; set; } = Brushes.LightGray;
        static TabbarItem _current;

        SvgIcon _icon = new SvgIcon { Width = 20 };
        TextBlock _text = new TextBlock { 
            Margin = new Thickness(0, 3, 0, 0),
            FontSize = 12,
            TextAlignment = TextAlignment.Center,
            TextWrapping = TextWrapping.Wrap,
        };
        public string Source
        {
            get => _icon.Source;
            set => _icon.Source = value;
        }
        public string Text
        {
            get => _text.Text;
            set => _text.Text = value;
        }
        public string Url { get; set; }


        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "Activated")
            {
                SetAnimation();

                if (e.NewValue.Equals(true))
                {
                    _current?.SetValue(ActivatedProperty, false);
                    _current = this;
                }
            }
            base.OnPropertyChanged(e);
        }

        void SetAnimation()
        {
            var value = Activated;
            var color = NormalColor;
            if (value)
            {
                _icon.Fill = color = ActiveColor;
                _text.Foreground = ActiveColor;
            }
            else
            {
                _icon.Fill = NormalColor;
                _text.Foreground = Brushes.DarkGray;
            }
            _icon.Stroke = color;
        }

        public TabbarItem()
        {
            Children.Add(_icon);
            Children.Add(_text);

            Activated = false;
            SetAnimation();

            this.MouseMove += (s, e) => {
                _icon.Stroke = ActiveColor;
                Background = Brushes.White;
            };
            this.MouseLeave += (s, e) => {
                Background = Brushes.Transparent;
            };
            this.RegisterClickEvent(() => {
                Activated = true;
                System.Mvc.Engine.Execute(Url);
            });
        }
    }
}
