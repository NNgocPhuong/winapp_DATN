using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    internal class PhoneNumberService : DeviceService
    {
        public int Sms { get => GetValue<int>("sms"); set => Push("sms", value); }
        public int Call { get => GetValue<int>("call"); set => Push("call", value); }

        public void Switch(string name)
        {
            this.Action = null;
            this.Push(name, this.GetValue<int>(name) ^ 1);
            
            Execute();
        }

        public void AddNumber(Station station, Document doc, Action completed)
        {
            Completed += completed;

            Station = station;
            BeginInsert(doc);
        }

        public override void RaiseCompleted()
        {
            Station.SelectContext("phone", map => {
                if (Action == "-")
                    map.Remove(Phone);
                else
                    map.Push(Phone, new Document().Copy(this, new string[] { "sms", "call" }));
            });
            base.RaiseCompleted();
        }
        public PhoneNumberService(string aname = "phone") : base(aname + "number")
        {
        }
    }
}
