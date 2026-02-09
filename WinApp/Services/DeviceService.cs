using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class DeviceService : Service
    {
        public Models.Station Station { get; set; }
        public DeviceService(string aname) : base("manager", "device", aname) { }

        protected override void BeginExecute()
        {
            ObjectId = Station.ObjectId;
        }
    }

    public class RemoteService : DeviceService
    {
        public RemoteService(Models.Station station) : base("remote")
        {
            Station = station;
        }
    }
}
