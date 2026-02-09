using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    using BsonData;
    public static class DB
    {
        static public MainDatabase Main { get; private set; }
        static public void Start(string path)
        {
            Main = new MainDatabase("MainDB");
            Main.Connect(path);
        }
        static public void End()
        {
            Main.Disconnect();
        }

        static public Collection Accounts => Main.GetCollection("accounts");
    }
}

namespace System
{
    public static class Global
    {
        static public AppUserModel User { get; set; }
        static public StationsList Stations { get; set; }
        static public PeopleList Persons { get; set; }
        static public DocumentList Alarms { get; set; }

        static public event Action<string, AlarmMessage> AlarmReceived;
        static public void OnAlarm(string id, Document data)
        {
            var s = Stations.FindOne(id);
            if (s != null) 
            {
                var message = s.ProcessAlarm(data);
                Alarms.Add(message);

                AlarmReceived?.Invoke(id, message);
            }
        }
    }
}
