
using CloudinaryDotNet.Actions;

namespace API.SignalR
{
    public class PresenceTracker
    {
        //temporary solution, store presence in memory
        //dict is the username, and then list of connectionids for user
        private static readonly Dictionary<string, List<string>> OnlineUsers =
            new Dictionary<string, List<string>>();

        public Task<bool> UserConnected(string username, string connectionid)
        {
            bool isOnlineChanged = false;
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
                    isOnlineChanged = true;
                }
            }

            return Task.FromResult(isOnlineChanged);
        }

        public Task<bool> UserDisconnected(string username, string connectionid)
        {
            bool isOfflineChanged = false;
            lock(OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOfflineChanged);

                OnlineUsers[username].Remove(connectionid);

                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                    isOfflineChanged = true;
                }

                return Task.FromResult(isOfflineChanged);
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

        public static Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionIds;

            //To make this scalable we will have to turn this into database rather than our dictionary in memory
            lock (OnlineUsers)
            {
                connectionIds = OnlineUsers.GetValueOrDefault(username);
            }

            return Task.FromResult(connectionIds);
        }
    }
}