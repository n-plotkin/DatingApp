using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using API.Interfaces;
using API.SignalR;
using API.Data;
using API.Services;
using API.Entities;
using API.Helpers;
using System.Text.Json;
using API.Helpers.Song;

public class SpotifyPollingService : BackgroundService
{
    // protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    // {
    //     while (!stoppingToken.IsCancellationRequested)
    //     {
    //         try
    //         {
    //             Console.WriteLine("Hello");
    //         }
    //         catch (Exception ex)
    //         {
    //             Console.WriteLine($"Error: {ex.Message}");
    //         }
    //         await Task.Delay(5000, stoppingToken);
    //     }
    // }

    private readonly PresenceTracker _presenceTracker;
    private readonly ISpotifyService _spotifyService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISpotifyAccountService _spotifyAccountService;

    private readonly IHubContext<SpotifyHub> _spotifyHubContext;
    private readonly IHubContext<PresenceHub> _presenceHub;


    public SpotifyPollingService(PresenceTracker presenceTracker,
     ISpotifyService spotifyService, IServiceProvider serviceProvider,
     ISpotifyAccountService spotifyAccountService,
     IHubContext<SpotifyHub> spotifyHubContext,
     IHubContext<PresenceHub> presenceHub)
    {
        _spotifyHubContext = spotifyHubContext;
        _spotifyAccountService = spotifyAccountService;
        _serviceProvider = serviceProvider;
        _spotifyService = spotifyService;
        _presenceTracker = presenceTracker;
        _presenceHub = presenceHub;

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {

            var onlineUsers = await _presenceTracker.GetOnlineUsersWithPresence();

            foreach (var u in onlineUsers)
            {
                Console.WriteLine($"Hello {u.Value.User.UserName}!");
                var user = await _presenceTracker.UpdateUser(u.Value.User.UserName);
                Console.WriteLine($"Got here?");


                Console.WriteLine($"Hello {user.UserSpotifyData == null}!");


                if (user.UserSpotifyData != null)
                {

                    Console.WriteLine($"NOW: {DateTime.UtcNow}, EXPIRES: {user.UserSpotifyData.ExpiresAt}");
                    if (DateTime.UtcNow >= user.UserSpotifyData.ExpiresAt)
                    {
                        await _spotifyAccountService.Refresh(user.UserName, "username");
                    }
                    Console.WriteLine($"NEW NOW: {DateTime.UtcNow}, EXPIRES: {user.UserSpotifyData.ExpiresAt}");

                    Console.WriteLine($"TOKEN: {user.UserSpotifyData.AccessToken}");
                    var currentlyPlaying = await _spotifyService.GetCurrentlyPlayingTrack(user.UserSpotifyData.AccessToken);
                    if (currentlyPlaying != null)
                    {
                        var currentartists = JsonSerializer.Serialize(currentlyPlaying.item.artists.Select(artist => artist.name).ToList());
                        if (currentlyPlaying.item.name != user.UserSpotifyData.CurrentSong
                            || currentartists != user.UserSpotifyData.CurrentArtists
                            ) // Check if there's a song playing and if it's a different song
                        {
                            //update user
                            var currentlyPlayingParameters = new CurrentlyPlaying
                            {
                                CurrentArtists = currentartists,
                                CurrentArtistsUris = JsonSerializer.Serialize(currentlyPlaying.item.artists.Select(artist => artist.uri).ToList()),
                                CurrentSong = currentlyPlaying.item.name,
                                CurrentSongUri = currentlyPlaying.item.album.images.FirstOrDefault().url
                            };
                            Console.WriteLine($"CurrentlyplayingParameters: {currentlyPlayingParameters.CurrentArtists.ToString()}");

                            Console.WriteLine("Sending to receiveSongUpdate");
                            Console.WriteLine("");


                            var username = u.Value.User.UserName; ;
                            if (SpotifyHub.UserConnectionMap.TryGetValue(username, out var connectionId))
                            {
                                //Find a better way of doing this..?
                                await _spotifyHubContext.Clients.All.SendAsync("receiveSongUpdate", "Updated.");
                                Console.WriteLine("Sent to receiveSongUpdate");
                            }
                            else
                            {
                                Console.WriteLine("User not connected.");
                            }
;



                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                                // Perform operations with unitOfWork here

                                // Commit changes
                                await unitOfWork.UserRepository
                                    .UpdateCurrentlyPlaying(user.UserName, currentlyPlayingParameters);
                                if (unitOfWork.HasChanges()) await unitOfWork.Complete();
                            }
                            Console.WriteLine("Updated database with new data");

                        }
                    }
                    else
                    {
                        Console.WriteLine("Got to else");

                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                            // Perform operations with unitOfWork here

                            var currentlyPlayingParameters = new CurrentlyPlaying
                            {
                                CurrentArtists = null,
                                CurrentArtistsUris = null,
                                CurrentSong = null,
                                CurrentSongUri = null
                            };


                            // Commit changes
                            await unitOfWork.UserRepository
                                .UpdateCurrentlyPlaying(user.UserName, currentlyPlayingParameters);
                            if (unitOfWork.HasChanges()) await unitOfWork.Complete();
                        }
                        Console.WriteLine("Updated database with empty");
                    }

                }

            }

            await Task.Delay(8000, stoppingToken); // Poll every 8 seconds
        }
    }
}
