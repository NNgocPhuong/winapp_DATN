using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WinApp
{

    public class Relay : Document
    {
        public int Index
        {
            get => GetValue<int>("index");
            set => Push("index", value);
        }
        public bool On
        {
            get => Status != 0;
            set => Value = (value ? 1 : 0);
        }
        new public int Status => (int)Value;
    }

    public class RelayViewModel : Document
    {
        public Relay Current { get; private set; }

        Relay _switcher;
        public RelayViewModel(int index = 0, int value = 0)
        {
            Current = new Relay { 
                Value = value,
                Index = index,
            };
        }

        public void BeginSwitch(Models.Station station)
        {
            if (_switcher != null) return;

            _switcher = new Relay {
                Index = Current.Index,
                Value = Current.On ? 0 : 1,
            };
            var service = new Services.RemoteService(station) { 
                Action = "Relay"
            };
            service.Execute(_switcher);
        }
        public RelayViewModel Update(int value)
        {
            if (Current.Status != value) {                
                return new RelayViewModel(Current.Index, value);
            }
            return this;
        }

        public Brush Color => (Current.On ? Brushes.Red : Brushes.Black);
        public string Text => (Current.On ? "ON" : "OFF");
    }
}
