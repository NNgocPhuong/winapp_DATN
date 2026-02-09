using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace WinApp
{
    public class ModelList : DocumentList
    {
        public AsyncViewModel Parent { get; private set; }
        public event Action<string, Document> Changed;
        public ModelList(AsyncViewModel parent)
        {
            this.Parent = parent;
        }
        public ModelList Load<T>(Action<T> action)
        {
            //foreach (Document e in Parent.ResponseContext.Items)
            //{
            //    var a = Activator.CreateInstance(typeof(T));
            //    base.Add(((Document)a).Copy(e));

            //    action?.Invoke((T)a);
            //}
            Changed?.Invoke("load", null);
            return this;
        }

        public void Update(string action, Document e)
        {
            switch (action)
            {
                case "-":
                    Remove(e);
                    break;

                case "+":
                    Add(e);
                    break;
            }
            Changed?.Invoke(action, e);
        }
        public void Update(Document e)
        {
            Update(e.Action, e);
        }
    }
}
