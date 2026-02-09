using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WinApp
{
    internal class MonitoringViewModel : AsyncViewModel
    {
        public ChartData ChartData { get; set; } = new ChartData();

        public Station Station
        {
            set => Global.Stations.Current = value;
            get => Global.Stations.Current;
        }
        public RelayViewModel Relay { get; set; }

        public event Action UpdateView;
        public override void Dispose()
        {
            Station.StopListenStatus();
            base.Dispose();
        }

        public override void Execute()
        {
        }

        public override bool IsReady => Station.IsReady;
        public override object GetDataContext()
        {
            return this;
        }
        public void Remote()
        {
            Relay.BeginSwitch(Station);
        }
        
        public void GetMonth(string date, Action<Document> completed)
        {
            var service = new MonthHistoryService { 
                Date = date,
            };
            service.Completed += () => {

                completed(service);
            };
            service.Execute();
        }

        
        public MonitoringViewModel()
        {
            Station.StatusChanged += (r) => {
                ChartData.Enqueue(r);

                int status = r.GetValue<int>("r");
                var relay = Relay;
                
                Relay = Relay?.Update(status) ?? new RelayViewModel(0, status);
                UpdateView?.Invoke();
            };

            Station.StartListenStatus();
            ChartData.Max = Station.GetValue<double>("iWarning");
        }


    }
}
