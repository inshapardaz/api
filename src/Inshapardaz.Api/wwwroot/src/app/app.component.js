var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Component } from '@angular/core';
import { ViewEncapsulation } from '@angular/core';
import { Router, NavigationStart } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { TranslateService } from '@ngx-translate/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { JwksValidationHandler } from 'angular-oauth2-oidc';
import { authConfig, AuthService } from '../services/auth.service';
var AppComponent = (function () {
    function AppComponent(router, auth, oauthService, translate, titleService) {
        var _this = this;
        this.router = router;
        this.auth = auth;
        this.oauthService = oauthService;
        this.translate = translate;
        this.titleService = titleService;
        this.currentRoute = "";
        this.isRtl = false;
        this.configureOAuth();
        this.setLanguages();
        this.router.events
            .subscribe(function (event) {
            if (event instanceof NavigationStart) {
                _this.currentRoute = event.url;
            }
        });
        this.translate.onLangChange.subscribe(function (event) {
            _this.isRtl = event.lang == 'ur';
        });
    }
    AppComponent.prototype.configureOAuth = function () {
        this.oauthService.configure(authConfig);
        this.oauthService.setupAutomaticSilentRefresh();
        this.oauthService.tokenValidationHandler = new JwksValidationHandler();
        this.oauthService.loadDiscoveryDocumentAndTryLogin();
    };
    AppComponent.prototype.setLanguages = function () {
        var _this = this;
        this.translate.addLangs(["en", "ur"]);
        this.translate.setDefaultLang('en');
        this.translate.get('APP.TITLE').subscribe(function (res) {
            _this.titleService.setTitle(res);
        });
        var browserLang = this.translate.getBrowserLang();
        var selectedLang = localStorage.getItem('ui-lang');
        this.translate.use(selectedLang ? selectedLang : (browserLang.match(/en|ur/) ? browserLang : 'en'));
        this.isRtl = selectedLang == 'ur';
    };
    return AppComponent;
}());
AppComponent = __decorate([
    Component({
        selector: 'my-app',
        templateUrl: './app.component.html',
        encapsulation: ViewEncapsulation.None
    }),
    __metadata("design:paramtypes", [Router,
        AuthService,
        OAuthService,
        TranslateService,
        Title])
], AppComponent);
export { AppComponent };
//# sourceMappingURL=app.component.js.map