using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinApp;

namespace Services
{
    public enum ResponseCode { 
        None,
        Success,
        Timeout,
    };
    public class RequestQueue : Vst.MQTT.Client
    {
        public RequestQueue Start(string id)
        {
            if (id != null) ID = id;

            Connect();
            return this;
        }
        public void Stop()
        {
            Disconnect();
        }

        //const string host = "system.aks.vn";
        const string host = "localhost";

        Queue<Service> _queue = new Queue<Service>();
        Service _currentModel;

        string _curentTopic;
        byte[] _currentPayload;
        int _repeatCount;
        int _remainCount;

        public void BeginRequest()
        {
            _remainCount = _currentModel.GetTimeout();
            Publish(_curentTopic, _currentPayload);
        }
        public void EndRequest()
        {
            _remainCount = 0;
            _repeatCount = 0;
            _currentModel = null;
        }

        Document _responseMap = new Document();
        public void RegisterResponse(string key, Service model)
        {
            _responseMap.Push(key, model);
        }

        public RequestQueue Enqueue(Service model)
        {
            _queue.Enqueue(model);
            return this;
        }
        public RequestQueue() : base(host)
        {
            this.SetCheckConnectionInterval(55);

            App.SysClock.OnTick += () => {

                if (_queue.Count == 0
                    || IsConnected == false
                    || _remainCount > 0
                    || _repeatCount > 0) return;

                if (_currentModel == null)
                {
                    _currentModel = _queue.Dequeue();

                    _repeatCount = _currentModel.GetRepeatCount();

                    var url = $"{_currentModel.ControllerName}/{_currentModel.ActionName}"
                        .Replace("-", "").ToLower();

                    RegisterResponse(url, _currentModel);

                    _curentTopic = $"{_currentModel.ServerName}/{url}";
                    _currentPayload = _currentModel.ToString().UTF8();

                    BeginRequest();
                }
            };

            App.SysClock.OnSecond += () => {
                if (_currentModel != null)
                {
                    _remainCount--;
                    if (_remainCount <= 0)
                    {
                        _repeatCount--;
                        if (_repeatCount <= 0)
                        {
                            _currentModel.ProcessResponse(ResponseCode.Timeout, null);
                            _currentModel = null;
                        }
                        else
                        {
                            BeginRequest();
                        }
                    }
                }
            };
        }

        protected override void RaiseConnectError()
        {
            App.ShowError("Không kết nối được server");
            base.RaiseConnectError();
        }

        protected override void RaiseConnectionLost()
        {
            App.ShowError("Mất kết nối");
            base.RaiseConnectionLost();
        }

        protected override void RaiseConnected()
        {
            Subscribe($"response/{ID}");
            base.RaiseConnected();
        }

        protected override void RaiseDataRecieved(string topic, byte[] payload)
        {
            try
            {
                var context = Document.Parse(payload.UTF8());
                if (topic.EndsWith("status"))
                {
                    var id = topic.Substring(0, topic.Length - 7);
                    Models.Station.ProcessStatus?.Invoke(id, context);
                    return;
                }
                if (topic.StartsWith("alarm"))
                {
                    var id = topic.Substring(6);
                    Global.OnAlarm(id, context);
                    return;
                }

                var url = context.Url;

                _responseMap.TryGetValue(url, out var v);

                var model = (Service)v;
                if (model == _currentModel)
                    EndRequest();

                model.ProcessResponse(ResponseCode.Success, context);
            }
            catch (Exception ex)
            {
                App.ShowError(ex.Message);
            }
        }
    }
}
