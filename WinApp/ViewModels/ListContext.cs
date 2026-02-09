using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinApp;

namespace System
{
    public class ListContext : BaseViewModel
    {
        public object SelectedItem { get; set; }
        public DocumentList Columns { get => GetDocumentList("columns"); set => Push("columns", value); }
        public ListContext() { }
        public ListContext(string name)
        {
            Load(name);
        }
        public ListContext Load(string name)
        {
            Clear();
            using (var sr = new IO.StreamReader(DB.Main.DataPath("list.json")))
            {
                var content = sr.ReadToEnd();
                var doc = Document.Parse(content);
                Copy(doc.GetDocument(name));

                return this;
            }
        }

    }
}
