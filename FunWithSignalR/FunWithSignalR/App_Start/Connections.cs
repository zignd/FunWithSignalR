using FunWithSignalR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithSignalR
{
    public class Connections
    {
        public static void PutUsersOffline()
        {
            using (var db = new ZigChatContext())
            {
                foreach (var connection in db.Connections)
                    connection.IsOnline = false;

                db.SaveChanges();
            }
        }
    }
}
