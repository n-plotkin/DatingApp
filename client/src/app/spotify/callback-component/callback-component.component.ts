import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SpotifyOAuthService } from 'src/app/_services/spotify-oauth-service';

@Component({
  selector: 'app-callback-component',
  templateUrl: './callback-component.component.html',
  styleUrls: ['./callback-component.component.css']
})
export class CallbackComponent implements OnInit {
  constructor(private route: ActivatedRoute, 
      private authService: SpotifyOAuthService, private router: Router) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const code = params['code'];
      if (code) {
        console.log('code: ' + code);
        this.authService.handleAuth(code);
        this.router.navigate(['/']);
      }
      // Handle error or state mismatch scenarios
    });
  }
}
