using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp
{
    public interface IStation
    {
        Models.Station Station { get; }
    }
    internal class StationViewModel : IStation
    {
        public Models.Station Station { get; set; }
        public string ObjectId => Station.ObjectId;
        public string Name => Station.Name;
        public string Address => Station.Address;
    }
}
