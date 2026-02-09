using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Vst.Controls;

namespace WinApp.Views
{
    internal class MonitoringBlockHeader : HeaderText
    {
        public MonitoringBlockHeader()
        {
            HeaderSize = 6;
            Foreground = Brushes.Gray;
        }
    }
    internal class MonitoringBlockItemValueText : MonitoringBlockItemText
    {
        public MonitoringBlockItemValueText()
        {
            HeaderSize = 5;
            Foreground = Brushes.Gray;
        }
    }
    internal class MonitoringBlockItemText : HeaderText
    {
        public MonitoringBlockItemText() 
        {
            HeaderSize = 7;
            HorizontalAlignment = HorizontalAlignment.Center;
            Foreground = Brushes.Gray;
        }
    }
    internal class MonitoringBlockItem : MyPanel<StackPanel>
    {
        Document _config;
        HeaderText _value;
        TextBlock _unit;
        public MonitoringBlockItem(Document config)
        {
            _config = config;

            _unit = new TextBlock {
                Text = config.Caption,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            _value = new MonitoringBlockItemValueText {
                HeaderSize = 3,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            Margin = new Thickness(2);

            Children.Add(_value);
            Children.Add(_unit);

            Background = Brushes.White;

            DataContextChanged += (s, e) => { 
                if (DataContext is Document doc)
                {
                    var v = doc.GetValue<double>(_config.Name);
                    var t = string.Format("{0:" + _config.Format + '}', v);
                    _value.Text = $"{t} {_config.Unit}";
                }
            };
        }
    }
    internal class MonitoringBlock : MyPanel<StackPanel>
    {
        HeaderText _header;
        WrapPanel _wrapPanel;
        public void Render(Document config, string name)
        {
            config = config.GetDocument(name);

            _header = new MonitoringBlockHeader { 
                Text = config.Caption,
            };
            
            _wrapPanel = new WrapPanel();

            Children.Add( _header);
            Children.Add(_wrapPanel);

            foreach (var e in config.Items)
            {
                _wrapPanel.Children.Add(new MonitoringBlockItem(e));
            }
        }
    }
}
