using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using API.Extensions;
using System.Collections.Concurrent;

namespace API.SignalR
{
    [Authorize]
    public class SpotifyHub : Hub
    {
        public static readonly ConcurrentDictionary<string, string> UserConnectionMap = new ConcurrentDictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            var username = Context.User.GetUsername(); // Assumed you have a method to get the username
            UserConnectionMap[username] = Context.ConnectionId;
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var username = Context.User.GetUsername(); // Assumed you have a method to get the username
            UserConnectionMap.TryRemove(username, out _);
            await base.OnDisconnectedAsync(exception);
        }

    }

}