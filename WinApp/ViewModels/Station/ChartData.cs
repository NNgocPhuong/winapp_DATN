using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp
{
    internal class ChartData
    {
        LinkedList<Document> list = new LinkedList<Document>();
        public void Enqueue(Document context)
        {
            list.AddLast(context);
        }
        public Document LastRecord => list.Last?.Value;
        public Document FirstRecord => list.First?.Value;

        public double Max { get; set; }
        public double Min { get; set; }
        public Document[] GetData()
        {
            var lst = new DocumentList();
            foreach (var item in list)
            {
                var e = new Document
                {
                    Time = item.GetDateTime("t"),
                    Value = item.GetValue<double>("i"),
                };
                lst.Add(e);
            }
            return lst.ToArray();
        }
    }
}
