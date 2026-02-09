using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp
{
    internal class AlarmViewModel : StationViewModel
    {
        public AlarmViewModel() { }
        public AlarmViewModel(AlarmMessage message)
        {
            Station = message.Station;
            Time = message.Time;
            Code = message.Code;
        }
        public DateTime? Time { get; set; }
        public int Code { get; set; }
    }
}
