using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class AlarmMessage : BaseModel
    {
        new public string Address => Station?.Address;
        public string StationName => Station?.Name;
        internal Station Station { get; set; }
    }

    public abstract class BaseFWModel : BaseModel
    {
        public abstract AlarmMessage CreateAlarmMessage(Document context);
    }
}
