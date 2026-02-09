using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinApp;

namespace Models
{
    public class Station : BaseModel
    {

        public bool IsReady => _alarmSummary != null && _lastAlarm != null;

        #region History
        public void LoadHistory()
        {
            if (_alarmSummary == null)
            {
                var model = new SummaryHistoryService
                {
                    Station = this,
                };
                model.ResponseSuccess += (context) => {
                    context = context.ValueContext;

                    _alarmSummary = new DocumentList();
                    foreach (var k in context.Keys)
                    {
                        if (k == "0" || char.IsDigit(k[0]) == false)
                            continue;

                        var t = new StringTime(k);
                        var e = new Document {
                            ObjectId = k,
                            Date = $"Tháng {t.Month} / {t.Year}",
                            Total = context.GetValue<int>(k)
                        };
                        _alarmSummary.Add(e);
                    }
                };
                model.Execute();
            }
            if (_lastAlarm == null)
            {
                var model = new DayHistoryService
                {
                    Station = this,
                };
                model.ResponseSuccess += (context) => {
                    _lastAlarm = model.AlarmList;
                };
                model.Execute();
            }
        }
        public AlarmMessage AlarmMessage { get; private set; }

        public DocumentList AlarmSummary => _alarmSummary;
        DocumentList _alarmSummary;
        #endregion

        #region Alarm
        public DocumentList LastAlarm => _lastAlarm;
        DocumentList _lastAlarm;

        internal AlarmMessage ProcessAlarm(Document data)
        {
            var message = CreateAlarmMessage(data);
            AlarmMessage = message;

            if (_lastAlarm != null)
            {
                _lastAlarm.Add(message);
            }
            return message;
        }

        public Station ClearAlarm()
        {
            AlarmMessage = null;
            return this;
        }
        public Station Demo()
        {
            AlarmMessage = new AlarmMessage { 
                Time = DateTime.Now,
                Code = new Random().Next(3) + 1,
                Station = this,
            };
            return this;
        }

        public void ListenAlarm()
        {
            Service.Subscribe($"alarm/{ObjectId}");
        }
        public AlarmMessage CreateAlarmMessage(Document context)
        {
            var r = CreateRecord();
            var m = r.CreateAlarmMessage(context);
            m.Station = this;

            return m;
        }

        #endregion

        #region Status
        public void StartListenStatus()
        {
            var topic = $"{ObjectId}/status";
            
            Service.Subscribe(topic);
            ProcessStatus = (id, context) => {
                if (id == null || id == ObjectId)
                {
                    var r = CreateRecord();
                    r.Copy(context);

                    StatusChanged?.Invoke(CurrentStatus = r);
                }
            };
        }
        public void StopListenStatus()
        {
            var topic = $"{ObjectId}/status";
            Service.Unsubscribe(topic);
            ProcessStatus = null;
        }

        static public Action<string, Document> ProcessStatus;
        public event Action<BaseFWModel> StatusChanged;
        public BaseFWModel CurrentStatus { get; private set; }
        #endregion

        public BaseFWModel CreateRecord()
        {
            var model = this.MODEL;
            var type = Type.GetType($"Models.{model}MODEL");
            if (type != null)
            {
                return (BaseFWModel)Activator.CreateInstance(type);
            }

            return null;
        }

        public Document GetGUIConfig()
        {
            var doc = LoadGUIConfig("models");
            return doc.GetDocument(MODEL);
        }

        public DocumentList GetPhoneNumbers()
        {
            var phone = this.GetDocument("phone");
            var list = new DocumentList();

            foreach (var k in phone.Keys)
            {
                var one = new PhoneNumberService() {
                    Station = this,
                    Phone = k
                };

                list.Add(one.Copy(phone.GetDocument(k)));
            }
            return list;
        }
    }
}
