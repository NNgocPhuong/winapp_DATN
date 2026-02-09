using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AlarmService : Service
    {
        Station _station;
        public Station Station
        {
            set => _station = value;
            get => _station ?? Global.Stations.Current;
        }
        public AlarmService(string cname, string aname) : base("alarm", cname, aname)
        {
        }

        public Document Dictionary { get; private set; }
        public Document Alarms { get; private set; }

        protected virtual void ProcessAlarms() { }
        public override void RaiseResponseSuccess(Document context)
        {
            var doc = context.ValueContext;
            Dictionary = doc.GetDocument("dic");
            Alarms = doc.ValueContext;

            ProcessAlarms();
            base.RaiseResponseSuccess(context);
        }

        protected override void BeginExecute()
        {
            ObjectId = Station.ObjectId;
            MODEL = Station.MODEL;

            base.BeginExecute();
        }
    }
}
