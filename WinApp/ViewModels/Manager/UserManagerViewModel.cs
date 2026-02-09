using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace WinApp
{
    internal class UserManagerViewModel : AsyncViewModel
    {
        public Document Current => (Document)Table.SelectedItem;
        public DocumentList Persons => Global.Persons.ToList();
        public ListContext Table { get; set; }

        public DocumentList Stations => Global.Stations.GetUserStations(Current);
        public DocumentList FutureStations => Global.Stations.GetUserFutureStations(Current);

        public override object GetDataContext()
        {
            Table = new ListContext("user-list") {
                Value = Persons,
            };
            return this;
        }
        public override void Execute()
        {
            var service = new AccountService();
            service.LoadPersons(null);
        }
        public override bool IsReady => Global.Persons != null;

        public void AddUser(Document data, Action completed)
        {
            var service = new AccountService();
            service.ResponseError += (c, m) => { 
                App.ShowError(m);
            };
            service.AddUser(data, completed);
        }
        public void DeleteUser(Action completed)
        {
            var service = new AccountService();
            service.RemoveUser(Current, () => {
                Table.SelectedItem = null;
                completed?.Invoke();
            });

        }

        public void DeviceToUser(object station, Action<Document> completed)
        {
            var device = (Document)station;
            var service = new DeviceToUserService(Current, device);
            service.Completed += () => {
                completed.Invoke(device);
            };
            service.Execute();
        }
    }
}
