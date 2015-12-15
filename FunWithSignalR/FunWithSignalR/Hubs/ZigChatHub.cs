using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using FunWithSignalR.Models;

namespace FunWithSignalR.Hubs
{
    public class ZigChatHub : Hub
    {
        public void SendMessage(string userName, string message)
        {
            if (message.StartsWith("@"))
            {
                var pmUserName = message.Split(' ')[0].Substring(1);

                using (var db = new ZigChatContext())
                {
                    var pmConnection = db.Connections.Where(x => x.UserName.ToLower() == pmUserName && x.IsOnline).SingleOrDefault();

                    if (pmConnection != null)
                    {
                        Clients.Clients(new List<string> { Context.ConnectionId, pmConnection.ConnectionId }).UpdateChat(userName, message, true);
                        return;
                    }   
                }
            }

            Clients.All.UpdateChat(userName, message);
        }

        public void UsersOnline()
        {
            try
            {
                using (var db = new ZigChatContext())
                {
                    Clients.All.UpdateUsersOnline(new { Success = true, UsersOnline = db.Connections.Where(x => x.IsOnline).Select(x => x.UserName).ToArray() });
                }
            }
            catch (Exception ex)
            {
                Clients.All.UpdateUsersOnline(new { Success = false, ErrorMessage = ex.Message });
            }
        }

        public object ConnectUser(string userName)
        {
            try
            {
                using (var db = new ZigChatContext())
                {
                    // Check if there if a connection for the specified user name have ever been made
                    var existingConnection = db.Connections.Where(x => x.UserName.ToLower() == userName.ToLower()).SingleOrDefault();
                    
                    if (existingConnection != null)
                    {
                        // If there's an old connection only the connection id and the online status are changed.
                        existingConnection.ConnectionId = Context.ConnectionId;
                        existingConnection.IsOnline = true;
                    }
                    else
                    {
                        // If not, then a new connection is created
                        db.Connections.Add(new Connection { ConnectionId = Context.ConnectionId, UserName = userName, IsOnline = true });
                    }

                    db.SaveChanges();
                }

                UsersOnline();

                return new { Success = true };
            }
            catch (Exception ex)
            {
                return new { Success = false, ErrorMessage = ex.Message };
            }
        }

        public override Task OnReconnected()
        {
            using (var db = new ZigChatContext())
            {
                var connection = db.Connections.Where(x => x.ConnectionId == Context.ConnectionId).SingleOrDefault();

                if (connection == null)
                    throw new Exception("An attempt to reconnect a non tracked connection id have been made.");

                connection.IsOnline = true;
                db.SaveChanges();
            }

            UsersOnline();

            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            using (var db = new ZigChatContext())
            {
                var connection = db.Connections.Where(x => x.ConnectionId == Context.ConnectionId).SingleOrDefault();

                if (connection == null)
                    throw new Exception("An attempt to disconnect a non tracked connection id have been made.");

                connection.IsOnline = false;
                db.SaveChanges();
            }

            UsersOnline();

            return base.OnDisconnected(stopCalled);
        }
    }
}