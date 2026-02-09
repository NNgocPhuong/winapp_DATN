using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;

namespace System
{
    partial class Document
    {
        public string Date { get => GetString("t"); set => Push("t", value); }
        public int Total { get => GetValue<int>("total"); set => Push("total", value); }
    }
}

namespace Services
{
    public class HistoryService : AlarmService
    {
        public HistoryService(string aname) : base("history", aname)
        {
        }

        //public override void RaiseCompleted()
        //{
        //    Items.Update("load", null);
        //    base.RaiseCompleted();
        //}
    }

    internal class DayHistoryService : HistoryService
    {
        public DocumentList AlarmList { get; private set; }
        protected override void ProcessAlarms()
        {
            var r = Station.CreateRecord();
            var d = Alarms.Pop("t");
            var count = Alarms.Pop("0");

            AlarmList = new DocumentList();
            foreach (var t in Alarms.Keys)
            {
                var a = r.CreateAlarmMessage(Alarms.GetDocument(t));
                a.Station = Station;
                a.Date = $"{d} {t}";

                AlarmList.Add(a);
            }
        }
        public DayHistoryService() : base("day")
        {
        }
        //protected override void ProcessResponseCore()
        //{
        //    //var list = GetBindingList();
        //    var context = ResponseContext.ValueContext.ValueContext;
        //    var time = context.Pop("t");
        //    var count = context.Pop<long>("0");

        //    if (context.Count > 0)
        //    {
        //        var model = Station.CreateRecord();
        //        foreach (var t in context.Keys)
        //        {
        //            var item = context.GetDocument(t);
        //            item.Push("t", $"{time} {t}");

        //            //var msg = model.CreateAlarmMessage(item);
        //            //msg.Station = Station;

        //            //Items.Add(msg);
        //        }
        //    }
        //    base.ProcessResponseCore();
        //}
    }

    internal class MonthHistoryService : HistoryService
    {
        public string Month { get; set; }
        public DocumentList GetDayAlarm(string day)
        {
            var d = $"{Date}-{day}";
            var r = Station.CreateRecord();

            var doc = Alarms.GetDocument(day);
            var count = doc.Pop("0");

            var list = new DocumentList();
            foreach (var t in doc.Keys)
            {
                var a = r.CreateAlarmMessage(doc.GetDocument(t));
                a.Station = Station;
                a.Date = $"{d} {t}";

                list.Add(a);
            }
            return list;
        }
        protected override void ProcessAlarms()
        {
            var d = new StringTime(Date);

            Month = $"Tháng {d.Month}, {d.Year}";

            base.ProcessAlarms();
        }
        public MonthHistoryService() : base("month")
        {

        }
    }

    internal class SummaryHistoryService : HistoryService
    {
        public SummaryHistoryService() : base("summary") 
        { 
        }
    }
}
