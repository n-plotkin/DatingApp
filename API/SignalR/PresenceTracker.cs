
using API.Entities;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet.Actions;

namespace API.SignalR
{
    public class PresenceTracker
    {
        //temporary solution, store presence in memory
        //dict is the username, and then list of connectionids for user
        private static readonly Dictionary<string, UserPresence> OnlineUsers =
            new Dictionary<string, UserPresence>();
        private readonly IServiceProvider _serviceProvider;

        public PresenceTracker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

        }

        public async Task<bool> UserConnected(string username, string connectionid)
        {
            bool isOnlineChanged = false;

            var appuser = new AppUser();

            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                // Use unitOfWork here for database operations

                // Ensure you complete any operations that require unitOfWork within this block

                if (!OnlineUsers.ContainsKey(username))
                {
                    appuser = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
                }

                //dictionary is not threadsafe, so multiple concurrent accesses would be bad
                lock (OnlineUsers)
                {
                    if (OnlineUsers.ContainsKey(username))
                    {
                        OnlineUsers[username].ConnectionIds.Add(connectionid);
                    }
                    else
                    {
                        OnlineUsers.Add(username, new UserPresence { ConnectionIds = { connectionid }, User = appuser });

                        isOnlineChanged = true;
                    }
                }

                return isOnlineChanged;
            }


        }

        public async Task<AppUser> UpdateUser(string username)
        {

            var appuser = new AppUser();
            Console.WriteLine($"Update user called on {username}...");
            Console.WriteLine($"Does online users contain username? {OnlineUsers.ContainsKey(username)}");


            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                Console.WriteLine($"Got inside scope");

                // Use unitOfWork here for database operations

                // Ensure you complete any operations that require unitOfWork within this block

                if (OnlineUsers.ContainsKey(username))
                {
                    Console.WriteLine($"Got inside containskey");

                    appuser = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
                    Console.WriteLine($"appuser {appuser.UserName} has spotifydata = {appuser.UserSpotifyData}");

                    //dictionary is not threadsafe, so multiple concurrent accesses would be bad
                    if (appuser.UserSpotifyData != OnlineUsers.GetValueOrDefault(username).User.UserSpotifyData)
                    {
                        lock (OnlineUsers)
                        {
                            Console.WriteLine($"Got inside lock?");
                            var connectionIds = OnlineUsers.GetValueOrDefault(username).ConnectionIds;

                            OnlineUsers.Remove(username);
                            OnlineUsers.Add(username, new UserPresence { ConnectionIds = connectionIds, User = appuser });
                            return appuser;
                        }
                    }
                }
            }
            return appuser;
        }


        public Task<bool> UserDisconnected(string username, string connectionid)
        {
            bool isOfflineChanged = false;
            lock (OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOfflineChanged);

                OnlineUsers[username].ConnectionIds.Remove(connectionid);

                if (OnlineUsers[username].ConnectionIds.Count == 0)
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
            lock (OnlineUsers)
            {
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }

        public static Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionIds = new List<string>();

            // Safely check if the username exists and only then access ConnectionIds
            if (OnlineUsers.TryGetValue(username, out var userPresence) && userPresence != null)
            {
                connectionIds = userPresence.ConnectionIds;
            }

            return Task.FromResult(connectionIds);

        }

        public Task<Dictionary<string, UserPresence>> GetOnlineUsersWithPresence()
        {
            Dictionary<string, UserPresence> usersWithPresence;
            lock (OnlineUsers)
            {
                // Clone the dictionary to avoid returning a reference to the original
                usersWithPresence = new Dictionary<string, UserPresence>(OnlineUsers);
            }
            return Task.FromResult(usersWithPresence);
        }
        public Task UpdateUserPresence(string username, AppUser user)
        {
            lock (OnlineUsers)
            {
                if (OnlineUsers.TryGetValue(username, out var userPresence))
                {
                    userPresence.User = user;
                }
                else
                {
                    // Optionally, handle the case where the user is not found
                    // For example, you could log this situation or throw an exception
                }
            }

            return Task.CompletedTask;
        }

    }
}