import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.html'
})
export class ProfileComponent implements OnInit {

  profile: any;

  constructor(public auth: AuthService) { }

  ngOnInit() {

    this.auth.refreshToken();
    if (this.auth.currentUser) {
      this.profile = this.auth.currentUser;
    } else {
      this.profile = this.auth.getUser();
    }
  }

}