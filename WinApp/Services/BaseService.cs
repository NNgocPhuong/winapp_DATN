using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class Service : BaseModel
    {
        public Service() { }
        public Service(string serverName) : this(serverName, null, null) { }
        public Service(string sname, string cname) : this(sname, cname, null) { }
        public Service(string sname, string cname, string aname)
        {
            ServerName = sname;
            ControllerName = cname;
            ActionName = aname;
        }

        public Service CreateActionService(string actionName, Document param)
        {
            var model = new Service(ServerName, ControllerName, actionName);
            if (param != null) param.Move(model);

            model.ResponseError += RaiseResponseError;
            //model.ResponseSuccess += RaiseResponseSuccess;
            //model.Completed += RaiseCompleted;

            return model;
        }

        #region PROPERTIES
        public bool Busy { get; protected set; }
        public string ServerName { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        #endregion

        #region Request
        static RequestQueue _engine;
        public RequestQueue Engine
        {
            get
            {
                if (_engine == null)
                    _engine = new RequestQueue();
                return _engine;
            }
        }

        public static void Subscribe(string topic) => _engine.Subscribe(topic);
        public static void Unsubscribe(string topic) => _engine?.Unsubscribe(topic);

        public virtual int GetRepeatCount() => 3;
        public virtual int GetTimeout() => 3;

        protected virtual void BeginExecute() { }
        public void Execute(Document param)
        {
            param?.Move(this);

            BeginExecute();
            Engine.Enqueue(this);
        }
        public void Execute() => Execute(null);
        public virtual void ProcessResponse(ResponseCode code, Document context)
        {
            if (code == ResponseCode.Timeout)
            {
                return;
            }
            if (code == ResponseCode.Success)
            {
                Console.WriteLine(context.ToString());

                var responseCode = context.Code;
                if (responseCode == 0)
                {
                    ProcessResponseContext(context);
                    RaiseResponseSuccess(context);
                }
                else
                {
                    RaiseResponseError(responseCode, context.Message);
                }
                RaiseCompleted();
            }
        }
        protected virtual void ProcessResponseContext(Document context)
        {

        }

        public virtual void RaiseCompleted() => Completed?.Invoke();
        public event Action Completed;

        public virtual void RaiseResponseSuccess(Document context) => ResponseSuccess?.Invoke(context);
        public event Action<Document> ResponseSuccess;

        public virtual void RaiseResponseError(int code, string message) => ResponseError?.Invoke(code, message);
        public event Action<int, string> ResponseError;
        #endregion

        #region Update
        public void BeginDelete(Document context = null)
        {
            Action = "-";
            Execute(context);
        }
        public void BeginInsert(Document context = null)
        {
            Action = "+";
            Execute(context);
        }
        #endregion
    }
}
