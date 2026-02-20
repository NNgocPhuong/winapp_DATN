using System;

namespace Services
{
    public class DeviceToUserService : Service
    {
        // Server mới: account/device/to-user
        public DeviceToUserService(Document user, Document device)
            : base("account", "device", "to-user")
        {
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
