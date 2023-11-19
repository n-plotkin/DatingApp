
using CloudinaryDotNet.Actions;

namespace API.SignalR
{
    public class PresenceTracker
    {
        //temporary solution, store presence in memory
        //dict is the username, and then list of connectionids for user
        private static readonly Dictionary<string, List<string>> OnlineUsers =
            new Dictionary<string, List<string>>();

        public Task UserConnected(string username, string connectionid)
        {
            //dictionary is not threadsafe, so multiple concurrent accesses would be bad
            lock(OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionid);
                }
                else
                {
                    OnlineUsers.Add(username, new List<string>{connectionid});
                }
            }

            return Task.CompletedTask;
        }

        public Task UserDisconnected(string username, string connectionid)
        {
            lock(OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(username)) return Task.CompletedTask;

                OnlineUsers[username].Remove(connectionid);

                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                }

                return Task.CompletedTask;
            }
        }
        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock(OnlineUsers)
            {
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }
    }
}