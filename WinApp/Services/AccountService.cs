using Models;
using System;

namespace Services
{
    public class LoginService : AccountService
    {
        public LoginService()
        {
            ActionName = "login";
        }
    }

    public class AccountService : Service
    {
        public AccountService() : base("account", "me")
        {
        }

        public void UpdateProfile(Document doc, Action completed)
        {
            var model = CreateActionService("update-profile", doc);
            model.Completed += () => {
                doc.Move(Global.User.Profile);
                completed?.Invoke();
            };
        }

        public void LoadStations(Action completed)
        {
            if (Global.Stations != null)
            {
                return;
            }

            // topic: account/device/list
            var service = new Service("account", "device", "list");
            service.ResponseError += RaiseResponseError;

            service.ResponseSuccess += (res) => {
                Global.Stations = new StationsList();
                Global.Alarms = new DocumentList();
                res.Items.ForEach(a => {
                    Global.Stations.Add(a);
                });

                completed?.Invoke();
            };

            service.Execute();
        }

        public void LoadPersons(Action completed)
        {
            if (Global.Persons != null)
            {
                return;
            }

            // topic: account/user/list
            var service = new Service("account", "user", "list");
            service.ResponseError += RaiseResponseError;

            service.ResponseSuccess += (res) => {
                Global.Persons = new PeopleList();
                Global.Persons.LoadResponseContextItems(res);
                completed?.Invoke();
            };

            service.Execute();
        }

        public void AddUser(Document data, Action completed, string role = "Staff")
        {
            // topic: account/user/add
            var service = new Service("account", "user", "add");
            service.ResponseError += RaiseResponseError;

            data?.Move(service);
            service.Role = role;

            service.ResponseSuccess += res => {

                var id = res.ValueContext.ObjectId;

                data.ObjectId = id;

                Global.Persons.Push(id, data);
                completed?.Invoke();
            };

            service.Execute();
        }

        public void RemoveUser(Document data, Action completed)
        {
            // topic: account/user/remove
            var service = new Service("account", "user", "remove");
            service.ResponseError += RaiseResponseError;

            service.ObjectId = data.ObjectId;

            service.ResponseSuccess += res => {
                Global.Persons.Remove(data.ObjectId);
                completed?.Invoke();
            };

            service.Execute();
        }
    }
}
