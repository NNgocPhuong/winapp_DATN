using System;
using System.Collections.Generic;
using System.Linq;
using System.Mvc;
using System.Text;
using System.Threading.Tasks;
using WinApp;

namespace Models
{
    public class BaseModel : Document
    {
    }

    public class ListModel : BaseModel
    {
        public void LoadResponseContextItems(Document context)
        {
            Clear();
            context.Items.ForEach(a => Add(a.ObjectId, a));
        }

        public DocumentList ToList()
        {
            var list = new DocumentList();
            foreach (Document e in this.Values) list.Add(e);

            return list;
        }

        public ListModel() { }
    }
}
