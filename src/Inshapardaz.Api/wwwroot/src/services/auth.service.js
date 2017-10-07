var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import 'rxjs/add/operator/filter';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Http, Headers, RequestOptions } from '@angular/http';
import { OAuthService } from 'angular-oauth2-oidc';
var authority = "http://ipid.azurewebsites.net";
var sessionOverride = sessionStorage.getItem('auth-server-address');
if (sessionOverride !== null) {
    authority = sessionOverride;
}
export var authConfig = {
    issuer: authority,
    redirectUri: window.location.origin,
    silentRefreshRedirectUri: window.location.origin + '/silent-renew.html',
    clientId: 'angular2client',
    scope: 'openid',
    sessionChecksEnabled: true,
    requireHttps: false
};
var AuthService = (function () {
    function AuthService(http, router, oauthService) {
        this.http = http;
        this.router = router;
        this.oauthService = oauthService;
    }
    AuthService.prototype.login = function () {
        this.oauthService.initImplicitFlow();
    };
    AuthService.prototype.logout = function () {
        this.oauthService.logOut();
        this.router.navigate(['/home']);
        return true;
    };
    AuthService.prototype.isAuthenticated = function () {
        return this.oauthService.getAccessToken() != null;
    };
    AuthService.prototype.refreshToken = function () {
        this.oauthService.silentRefresh();
    };
    AuthService.prototype.loadUserProfile = function () {
        var _this = this;
        this
            .oauthService
            .loadUserProfile()
            .then(function (up) {
            console.log(up);
            _this.userProfile = up;
        });
    };
    AuthService.prototype.AuthGet = function (url, options) {
        if (options) {
            options = this._setRequestOptions(options);
        }
        else {
            options = this._setRequestOptions();
        }
        return this.http.get(url, options);
    };
    AuthService.prototype.AuthPut = function (url, data, options) {
        var body = JSON.stringify(data);
        if (options) {
            options = this._setRequestOptions(options);
        }
        else {
            options = this._setRequestOptions();
        }
        return this.http.put(url, body, options);
    };
    AuthService.prototype.AuthDelete = function (url, options) {
        if (options) {
            options = this._setRequestOptions(options);
        }
        else {
            options = this._setRequestOptions();
        }
        return this.http.delete(url, options);
    };
    AuthService.prototype.AuthPost = function (url, data, options) {
        var body = JSON.stringify(data);
        if (options) {
            options = this._setRequestOptions(options);
        }
        else {
            options = this._setRequestOptions();
        }
        return this.http.post(url, body, options);
    };
    AuthService.prototype._setAuthHeaders = function () {
        var authHeaders = new Headers();
        authHeaders.append("Authorization", "Bearer " + this.oauthService.getIdToken());
        if (authHeaders.get('Content-Type')) {
        }
        else {
            authHeaders.append('Content-Type', 'application/json');
        }
        return authHeaders;
    };
    AuthService.prototype._setRequestOptions = function (options) {
        var authHeaders;
        if (this.isAuthenticated()) {
            authHeaders = this._setAuthHeaders();
        }
        if (options) {
            options.headers.append(authHeaders.keys[0], authHeaders.values[0]);
        }
        else {
            options = new RequestOptions({ headers: authHeaders });
        }
        return options;
    };
    return AuthService;
}());
AuthService = __decorate([
    Injectable(),
    __metadata("design:paramtypes", [Http,
        Router,
        OAuthService])
], AuthService);
export { AuthService };
//# sourceMappingURL=auth.service.js.map