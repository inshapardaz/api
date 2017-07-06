import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import 'rxjs/add/operator/filter';
import {Observable} from 'rxjs/Observable';
import * as auth0 from 'auth0-js';

@Injectable()
export class AuthService {
  _auth0 = new auth0.WebAuth({
    clientID: 'WkEHQXUHgcec5GhzLqUZ0PTVYJ4u9ihI',
    domain: 'inshapardaz.eu.auth0.com',
    responseType: 'token id_token',
    audience: 'https://inshapardaz.eu.auth0.com/userinfo',
    redirectUri: "http://" + window.location.hostname + "/callback",      
    scope: 'openid'
  });

  refreshSubscription : any;
  userProfile: any;

  constructor(public router: Router) {
  }

  ngOnInit() {
    if (this.isAuthenticated()) {
      this.getProfile(() => console.log('profile loaded'));
    }
  }


  public login(): void {
    this._auth0.authorize({});
  }

  public handleAuthentication(): void {
    this._auth0.parseHash((err, authResult) => {
      if (authResult && authResult.accessToken && authResult.idToken) {
        window.location.hash = '';
        this.setSession(authResult);
        this.getProfile(() => this.router.navigate(['/home']));
      } else if (err) {
        this.router.navigate(['/home']);
        console.log(err);
      }
    });
  }

  private setSession(authResult): void {
    const expiresAt = JSON.stringify((authResult.expiresIn * 1000) + new Date().getTime());
    localStorage.setItem('access_token', authResult.accessToken);
    localStorage.setItem('id_token', authResult.idToken);
    localStorage.setItem('expires_at', expiresAt);

    this.scheduleRenewal();
  }

  public logout(): boolean {
    localStorage.removeItem('access_token');
    localStorage.removeItem('id_token');
    localStorage.removeItem('expires_at');

    this.unscheduleRenewal();
    
    this.router.navigate(['/home']);
    return true;
  }

  public isAuthenticated(): boolean {
    const expiresAt = JSON.parse(localStorage.getItem('expires_at'));
    return new Date().getTime() < expiresAt;
  }

  public renewToken() {
    this._auth0.renewAuth({
      audience: 'https://inshapardaz.eu.auth0.com/userinfo',
      redirectUri: this.relPathToAbs('/silent'),
      usePostMessage: true
    }, (err, result) => {
      if (err) {
        alert(`Could not get a new token using silent authentication (${err.error}).`);
      } else {
        alert(`Successfully renewed auth!`);
        this.setSession(result);
      }
    });
  }

  public scheduleRenewal() {
    if(!this.isAuthenticated()) return;
    this.unscheduleRenewal();

    const expiresAt = JSON.parse(window.localStorage.getItem('expires_at'));

    const source = Observable.of(expiresAt).flatMap(
      expiresAt => {

        const now = Date.now();

        // Use the delay in a timer to
        // run the refresh at the proper time
        console.log(expiresAt - now);
        return Observable.timer(Math.max(1, expiresAt - now));
      });

    // Once the delay time from above is
    // reached, get a new JWT and schedule
    // additional refreshes
    this.refreshSubscription = source.subscribe(() => {
      this.renewToken();
      this.scheduleRenewal();
    });
  }

  public unscheduleRenewal() {
    if(!this.refreshSubscription) return;
    this.refreshSubscription.unsubscribe();
  }

  public getProfile(cb): void {
    var accessToken = localStorage.getItem('access_token');
    if (accessToken == null) {
      cb(null, {});
      return;
    }

    const self = this;
    this._auth0.client.userInfo(accessToken, (err, profile) => {
      if (profile) {
        self.userProfile = profile;
      }
      cb(err, profile);
    });
  }

  private relPathToAbs (sRelPath) : string {
    var nUpLn, sDir = "", sPath = location.pathname.replace(/[^\/]*$/, sRelPath.replace(/(\/|^)(?:\.?\/+)+/g, "$1"));
    for (var nEnd, nStart = 0; nEnd = sPath.indexOf("/../", nStart), nEnd > -1; nStart = nEnd + nUpLn) {
      nUpLn = /^\/(?:\.\.\/)*/.exec(sPath.slice(nEnd))[0].length;
      sDir = (sDir + sPath.substring(nStart, nEnd)).replace(new RegExp("(?:\\\/+[^\\\/]*){0," + ((nUpLn - 1) / 3) + "}$"), "/");
    }
    return sDir + sPath.substr(nStart);
  }
}