using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Models
{
    public class UserModel : BaseModel
    {
        public UserModel(Document context)
        {
            this.Copy(context);
        }
        public StationsList Stations { get; private set; } = new StationsList();
    }
}
