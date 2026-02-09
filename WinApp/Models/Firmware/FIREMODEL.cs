using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WinApp;

namespace Models
{
    public class FIREMODEL : BaseFWModel
    {
        static string[] _names = new string[] { "", "Quá tải điện", "Chập điện", "Báo cháy" };

        public override AlarmMessage CreateAlarmMessage(Document context)
        {
            
            var code = context.GetValue<int>("s");
            return new AlarmMessage {
                Name = _names[code],
                Time = context.Time,
                Code = code,
            };
        }

        public double Current { get => GetValue<double>("i"); }
        public int Voltage { get => GetValue<int>("u"); }
        public int MonthPower { get => GetValue<int>("p"); }
        public int Cos { get => GetValue<int>("f"); }
        public int ILeak { get => GetValue<int>("l"); }
        public int MonthCost { get => GetValue<int>("m"); }
        public int DayPower { get => GetValue<int>("d"); }
        public int Fire { get => GetValue<int>("z"); }
    }
}
