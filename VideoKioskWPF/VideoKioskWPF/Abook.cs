using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoKioskWPF
{
    public enum Status
    {
        USER_INVALID = -1,
        USER_LOGOFF = 0,
        USER_AVAIL = 1,
        USER_BUSY = 2,
        USER_MULTIHOST = 5
    }
    public class User
    {
        public string ID { get; set; }
        public string DN { get; set; }
        public Status Status { get; set; }
        
        public User() { }
        public User(string id, string dn, int status)
        {
            ID = id;
            DN = dn;
            Status = (Status)status;
        }
    }
    class Abook
    {
        public List<User> users;
        Random r = new Random();

        public Abook()
        {
            users = new List<User>();
        }

        public void UpdateAbook(string json)
        {
            dynamic data = JsonConvert.DeserializeObject(json);
            if (data.abook != null)
                foreach (dynamic user in data.abook)
                {
                    string id = (string)user.peerId;
                    string dn = (string)user.peerDn;
                    int st = (int)user.status;
                    if (users.Exists(c => c.ID == id))
                    {
                        users.Find(c => c.ID == id).Status = (Status)st;
                        users.Find(c => c.ID == id).DN = dn;
                    }
                    else
                        users.Add(new User(id, dn, st));
                }
        }

        public string GetRandomOnlineUser()
        {
            List<User> onlineUsers = users.FindAll(c => c.Status == Status.USER_AVAIL);
            if (onlineUsers.Count <= 0)
            {
                return null;
            }
            else
            {
                int index = r.Next(onlineUsers.Count - 1);
                return onlineUsers.ElementAt(index).ID;
            }
        }
    }
}
