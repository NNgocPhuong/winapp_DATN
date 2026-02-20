using System;

namespace Services
{
    public class DeviceService : Service
    {
        public Models.Station Station { get; set; }

        // Server mới: account/device/{action}
        public DeviceService(string aname) : base("account", "device", aname)
        {
        }

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
