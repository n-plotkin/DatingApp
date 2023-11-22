import { Injectable } from '@angular/core';
import { Observable, take } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { AccountService } from './account.service';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class SpotifyOAuthService {
  private redirectUri = 'https://localhost:4200/callback'; // Your redirect URI
  private scope =
    'user-read-private user-read-email ' +
    'user-read-recently-played ' +
    'user-top-read ' +
    'playlist-read-private ' +
    'user-read-currently-playing ' +
    'user-read-playback-state ' +
    'user-modify-playback-state ' +
    'playlist-read-collaborative ' +
    'playlist-read-private ' +
    'playlist-modify-public ' +
    'playlist-modify-private';

  private baseUrl = environment.apiUrl;
  private clientId = environment.clientId;

  constructor(private http: HttpClient) {}

  login(): void {
    const state = this.generateRandomString(16);
    const authUrl = `https://accounts.spotify.com/authorize?` +
      `client_id=${this.clientId}&` +
      `redirect_uri=${encodeURIComponent(this.redirectUri)}&` +
      `scope=${encodeURIComponent(this.scope)}&` +
      `response_type=code&` +
      `state=${state}`;
    console.log(authUrl);

    window.location.href = authUrl;
  }


  public handleAuth(code: string) {
    // This will be api endpoint yet to be built

    return this.http.post(this.baseUrl + 'spotify/auth', {code}).subscribe({
      next: response => {
        console.log(response)
      }
    });
  }


  private generateRandomString(length: number): string {
    let text = '';
    const possible = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';

    for (let i = 0; i < length; i++) {
      text += possible.charAt(Math.floor(Math.random() * possible.length));
    }

    return text;
  }
}
