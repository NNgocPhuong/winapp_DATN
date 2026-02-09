using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class StationsList : ListModel
    {
        public Station Current { get; set; }

        public Station FindOne(string id)
        {
            return (Station)this.GetDocument(id);
        }
        public Station Add(Document doc)
        {
            var s = new Station();

            Add(doc.ObjectId, s.Copy(doc));
            if (Current == null)
            {
                Current = s;
            }
            s.ListenAlarm();

            var log = s.GetDocument("alarm");
            if (log.Count > 0)
            {
                Global.Alarms.Add(s.CreateAlarmMessage(log));
            }
            return s;
        }

        public DocumentList GetUserStations(Document user)
        {
            if (user == null) return null;

            var lst = new DocumentList();
            user.SelectContext("device", map => {
                foreach (var key in map.Keys)
                {
                    var e = FindOne(key);
                    if (e != null)
                    {
                        lst.Add(e);
                        e.Action = "-";
                    }
                }
            });
            return lst;
        }
        public DocumentList GetUserFutureStations(Document user)
        {
            if (user == null) return null;

            var lst = new DocumentList();
            user.SelectContext("device", map => {
                foreach (var p in this)
                {
                    if (map.ContainsKey(p.Key) == false)
                    {
                        var e = (Station)p.Value;
                        e.Action = "+";
                        lst.Add(e);
                    }
                }
            });
            return lst;
        }
    }
}
