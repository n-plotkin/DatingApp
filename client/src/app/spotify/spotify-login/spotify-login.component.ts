import { Component, OnInit } from '@angular/core';
import { SpotifyOAuthService } from 'src/app/_services/spotify-oauth-service';

@Component({
  selector: 'app-spotify-login',
  templateUrl: './spotify-login.component.html',
  styleUrls: ['./spotify-login.component.css']
})
export class SpotifyLoginComponent implements OnInit {
  constructor(private authService: SpotifyOAuthService) {}

  ngOnInit(): void {
    this.login();
  }

  
  login(): void {
    this.authService.login();
  }

}
