using System;
using System.Collections.Generic;
using System.Linq;
using System.Mvc;
using System.Text;
using System.Threading.Tasks;
using WinApp;

namespace Models
{
    public class AppUserModel : BaseModel
    {
        public Document Profile => SelectContext("me", doc => { });
        public AppUserModel(string token, Document context)
        {
            Copy(context);
            var id = context.ObjectId;
            var doc = new Document {
                ObjectId = "#last",
                UserName = id,
                Token = token,
                Time = DateTime.Now,
            };
            DB.Accounts.InsertOrUpdate(doc);
        }
    }
}
