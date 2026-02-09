using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Vst.Controls;

namespace WinApp.Views
{
    internal interface IAlarmViewer { }
    internal abstract class AlarmViewer<T> : MyElement
        where T : FrameworkElement, new()
    {
        public T MainContent => (T)Child;

        void OnAlarmReceived(string id, AlarmMessage message)
        {
            Dispatcher.InvokeAsync(() => { 
                ProcessAlarm(id, message);
            });
        }
        abstract protected void ProcessAlarm(string id, AlarmMessage message);
        public AlarmViewer() 
        {
            Child = new T();
            this.IsVisibleChanged += (s, e) => { 
                if ((bool)e.NewValue)
                {
                    Global.AlarmReceived += OnAlarmReceived;
                }
                else
                {
                    Global.AlarmReceived -= OnAlarmReceived;
                }
            };
        }
    }
}
