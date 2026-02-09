using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class DeviceToUserService : AccountService
    {
        public DeviceToUserService(Document user, Document device)
        {
            ActionName = "device-to-user";

            Add("userId", user.ObjectId);
            Add("deviceId", device.ObjectId);

            Action = device.Action;
            Completed += () => {
                user.SelectContext("device", map => {
                    if (Action == "+")
                    {
                        map.Push(device.ObjectId, new Document());
                    }
                    else
                    {
                        map.Remove(device.ObjectId);
                    }
                });
            };
        }
    }
}
