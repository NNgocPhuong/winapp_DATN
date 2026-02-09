using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    partial class Document
    {
        static public Document LoadGUIConfig(string name)
        {
            using (var sr = new IO.StreamReader(DB.Main.DataPath($"{name}.json")))
            {
                var content = sr.ReadToEnd();
                var doc = Document.Parse(content);

                return doc;
            }
        }
    }
}

namespace System
{
    partial class Document
    {
        public string Caption { get => GetString("caption"); set => Push("caption", value); }
        public string Phone { get => GetString("phone"); set => Push("phone", value); }
        public string Address { get => GetString("addr"); set => Push("addr", value); }
        public string MODEL { get => GetString("model"); set => Push("model", value); }
        public string VERSION { get => GetString("version"); set => Push("version", value); }
        public string Unit { get => GetString("unit"); set => Push("unit", value); }
        public string Status { get => GetString("s"); set => Push("s", value); }
        public string Format { get => GetString("format"); set => Push("format", value); }
        public DateTime? Time { get => GetDateTime("t"); set => Push("t", value); }
    }
}
