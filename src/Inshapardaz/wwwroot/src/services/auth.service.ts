import 'rxjs/add/operator/filter';

import { Injectable, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';

import { AuthConfig, OAuthService } from 'angular-oauth2-oidc';

var authority : string = "http://ipid.azurewebsites.net"
var sessionOverride  : string = sessionStorage.getItem('auth-server-address');
if (sessionOverride !== null) {
  authority = sessionOverride;
}

export const authConfig: AuthConfig = { 
   issuer: authority,
   redirectUri: window.location.origin,
   silentRefreshRedirectUri: window.location.origin + '/silent-renew.html',
   clientId: 'angular2client',
   scope: 'openid',
   sessionChecksEnabled: true,
   requireHttps : false
 }


@Injectable()
export class AuthService {
  public userProfile : any;
  constructor(private http: Http, 
              public router: Router,
              private oauthService: OAuthService) {
  }

  public login() {
    this.oauthService.initImplicitFlow();
  }

  public logout(): boolean {
    this.oauthService.logOut();

    this.router.navigate(['/home']);
    return true;
  }

  public isAuthenticated(): boolean {
    return this.oauthService.getAccessToken() != null;
  }

  public refreshToken(){
    this.oauthService.silentRefresh();
  }

  public loadUserProfile(): void {
    this
        .oauthService
        .loadUserProfile()
        .then(up => {
          console.log(up);
          this.userProfile = up;
        });

  }
  
  AuthGet(url: string, options?: RequestOptions): Observable<Response> {

    if (options) {
      options = this._setRequestOptions(options);
    }
    else {
      options = this._setRequestOptions();
    }
    return this.http.get(url, options);
  }
  
  AuthPut(url: string, data: any, options?: RequestOptions): Observable<Response> {

    let body = JSON.stringify(data);

    if (options) {
      options = this._setRequestOptions(options);
    }
    else {
      options = this._setRequestOptions();
    }
    return this.http.put(url, body, options);
  }

  AuthDelete(url: string, options?: RequestOptions): Observable<Response> {

    if (options) {
      options = this._setRequestOptions(options);
    }
    else {
      options = this._setRequestOptions();
    }
    return this.http.delete(url, options);
  }
  
  AuthPost(url: string, data: any, options?: RequestOptions): Observable<Response> {

    let body = JSON.stringify(data);

    if (options) {
      options = this._setRequestOptions(options);
    } else {
      options = this._setRequestOptions();
    }
    return this.http.post(url, body, options);
  }


  private _setAuthHeaders(): Headers {
    var authHeaders = new Headers();
    authHeaders.append("Authorization", "Bearer " + this.oauthService.getIdToken());
    if (authHeaders.get('Content-Type')) {

    } else {
      authHeaders.append('Content-Type', 'application/json');
    }
    return  authHeaders;
  }
  private _setRequestOptions(options?: RequestOptions) {
    let authHeaders : Headers;
    if (this.isAuthenticated()) {
      authHeaders = this._setAuthHeaders();
    }
    if (options) {
      options.headers.append(authHeaders.keys[0], authHeaders.values[0]);
    } else {
      options = new RequestOptions({ headers: authHeaders });
    }

    return options;
  }

}

