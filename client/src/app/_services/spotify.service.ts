import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';
import { userSpotifyData } from '../_models/userSpotifyData';

@Injectable({
  providedIn: 'root'
})

export class SpotifyService {
  hubUrl = environment.hubUrl;
  baseUrl = environment.apiUrl;
  private hubConnection?: HubConnection;
  private currentSongSource = new BehaviorSubject<userSpotifyData | null>(null);
  currentSong$ = this.currentSongSource.asObservable();

  constructor(private toastr: ToastrService, private http: HttpClient) { }


  getCurrentData() {
     // Adjust the URL as needed
    this.http.get<string[]>(this.baseUrl + 'spotify/me') // Correct use of HttpClient with type argument
      .subscribe(
        data => {
          var spotifyData: userSpotifyData = {
            artist: data[0],
            artistImageUri: data[1],
            currentArtists: JSON.parse(data[2]),
            currentArtistsUris: JSON.parse(data[3]),
            song: data[4], 
            songUri: data[5]
          };
          this.currentSongSource.next(spotifyData);
           // On success, update the BehaviorSubject
        },
        error => {
          console.error('Error fetching Spotify data:', error); // Error handling
        }
      );
  }

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'spotify', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .catch(error => console.error('Error establishing the connection: ', error));

    this.hubConnection.on('receiveSongUpdate', _ => {
      console.log("Recieved update");
      this.getCurrentData();
      //this.handleSongUpdate(song, artists, songurl);
    });
  }

  stopHubConnection() {
    this.hubConnection?.stop().catch(error => console.error('Error stopping the connection: ', error));
  }

}
