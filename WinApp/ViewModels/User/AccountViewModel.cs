using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp
{
    //internal partial class AccountViewModel : ManagerAccountViewModel
    //{
    //    ModelList _stations;
    //    public ModelList Stations
    //    {
    //        get 
    //        {
    //            if (_stations == null)
    //            {
    //                var map = GetDocument("device").Clone();

    //                _stations = new ModelList(this);

    //                foreach (var e in App.Stations.Items)
    //                {
    //                    if (map.ContainsKey(e.ObjectId))
    //                    {
    //                        _stations.Add(e);

    //                        e.Action = "-";

    //                        map.Remove(e.ObjectId);

    //                        if (map.Count == 0)
    //                            break;
    //                    }
    //                }
    //            }
    //            return _stations; 
    //        }
    //    }

    //    public ModelList FreeStations()
    //    {
    //        var lst = new ModelList(this);
    //        var map = GetDocument("device");

    //        foreach (var e in App.Stations.Items)
    //        {
    //            if (!map.ContainsKey(e.ObjectId))
    //            {
    //                e.Action = "+";
    //                lst.Add(e);
    //            }
    //        }
    //        return lst;
    //    }
    //    public AccountViewModel(string action) : base(action) { }
    //    public AccountViewModel() : base("") { }

    //    public void SetDevice(StationViewModel station, bool enable, Action completed)
    //    {
    //        var model = new AccountViewModel("device-to-user") {
    //            { "deviceId", station.ObjectId },
    //            { "userId", ObjectId },
    //        };
    //        var action = enable ? "+" : "-";

    //        model.Completed += () => { 
    //            Stations.Update(action, station);
    //            completed?.Invoke();
    //        };

    //        model.BeginAction(action);
    //    }
    //    public void SetDevice(StationViewModel station, Action completed)
    //    {
    //        var model = new AccountViewModel("device-to-user") {
    //            { "deviceId", station.ObjectId },
    //            { "userId", ObjectId },
    //        };

    //        model.Completed += () => {
    //            _stations = null;

    //            SelectContext("device", doc => {
    //                doc.Remove(station.ObjectId);
    //                if (station.Action == "+")
    //                    doc.Add(station.ObjectId, new Document());
    //            });
    //            completed?.Invoke();
    //        };

    //        model.Enqueue();
    //    }

    //}
}
