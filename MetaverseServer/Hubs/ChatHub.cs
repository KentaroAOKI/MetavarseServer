using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

// Please refer to here for how to make it.
// https://docs.microsoft.com/ja-jp/aspnet/signalr/overview/getting-started/tutorial-getting-started-with-signalr-and-mvc

namespace MetaverseServer.Hubs
{
    public class State
    {
        public State()
        {
            UserName = string.Empty;
            VrmUrl = string.Empty;
            Message = string.Empty;
            PositionX = 0f;
            PositionY = 0f;
            PositionZ = 0f;
        }
        public string UserName { get; set; }
        public string VrmUrl { get; set; }
        public string Message { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
    }
    public class ChatHub : Hub
    {
        private static Dictionary<string, State> clients = new Dictionary<string, State>();

        public override Task OnConnected()
        {
            // Add your own code here.
            // For example: in a chat application, record the association between
            // the current connection ID and user name, and mark the user as online.
            // After the code in this method completes, the client is informed that
            // the connection is established; for example, in a JavaScript client,
            // the start().done callback is executed.
            System.Diagnostics.Debug.WriteLine("OnConnected:" + Context.ConnectionId);
            if (clients.ContainsKey(Context.ConnectionId) == false)
            {
                clients.Add(Context.ConnectionId, new State());
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            // Add your own code here.
            // For example: in a chat application, mark the user as offline, 
            // delete the association between the current connection id and user name.
            System.Diagnostics.Debug.WriteLine("OnDisconnected:" + Context.ConnectionId);
            if (clients.ContainsKey(Context.ConnectionId) == true)
            {
                clients.Remove(Context.ConnectionId);
                Clients.Others.destoryAvater(Context.ConnectionId);
            }
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            // Add your own code here.
            // For example: in a chat application, you might have marked the
            // user as offline after a period of inactivity; in that case 
            // mark the user as online again.
            System.Diagnostics.Debug.WriteLine("OnReconnected:" + Context.ConnectionId);
            if (clients.ContainsKey(Context.ConnectionId) == false)
            {
                clients.Add(Context.ConnectionId, new State());
            }
            return base.OnReconnected();
        }

        public void Join(string userName, string vrmUrl)
        {
            if (clients.ContainsKey(Context.ConnectionId) == true)
            {
                State state = clients[Context.ConnectionId];
                state.UserName = userName;
                state.VrmUrl = vrmUrl;
                Clients.Others.spawnAvater(Context.ConnectionId, state.UserName, state.VrmUrl, state.PositionX, state.PositionY, state.PositionZ);
            }
            // Call the setEnvironment method to update clients.
            Dictionary<string, State> resultClients = new Dictionary<string, State>(clients);
            resultClients.Remove(Context.ConnectionId);
            Clients.Caller.setEnvironment(resultClients);
        }
        public void SetMessage(string message)
        {
            // Call the setMessage method to update clients.
            if (clients.ContainsKey(Context.ConnectionId) == true)
            {
                State state = clients[Context.ConnectionId];
                state.Message = message;
                Clients.Others.setMessage(Context.ConnectionId, message);
            }
            System.Diagnostics.Debug.WriteLine("SetMessage:" + Context.ConnectionId);
        }
        public void SetPosition(float positionX, float positionY, float positionZ)
        {
            // Call the setPosition method to update clients.
            if (clients.ContainsKey(Context.ConnectionId) == true)
            {
                State state = clients[Context.ConnectionId];
                state.PositionX = positionX;
                state.PositionY = positionY;
                state.PositionZ = positionZ;
                Clients.Others.setPosition(Context.ConnectionId, positionX, positionY, positionZ);
            }
            System.Diagnostics.Debug.WriteLine("SetPosition:" + Context.ConnectionId);
        }
    }
}