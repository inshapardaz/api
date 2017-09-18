import { Component } from '@angular/core';
import { ViewEncapsulation } from '@angular/core';
import { RouterModule , Router, NavigationStart } from '@angular/router';
import { Title }     from '@angular/platform-browser';

import { TranslateService, LangChangeEvent} from 'ng2-translate/ng2-translate';
import { OAuthService } from 'angular-oauth2-oidc';
import { JwksValidationHandler } from 'angular-oauth2-oidc';

import { authConfig, AuthService} from '../services/auth.service';

@Component({
    selector: 'my-app',
    templateUrl: './app.component.html',
    encapsulation: ViewEncapsulation.None
})

export class AppComponent {
    currentRoute: string = "";
    isRtl : boolean = false;
    constructor(private router: Router, 
                private auth: AuthService, 
                private oauthService: OAuthService,
                private translate: TranslateService, 
                private titleService: Title) {
        this.configureOAuth();
        this.setLanguages();
        this.router.events
        .subscribe(event => {
            if (event instanceof NavigationStart){
                 this.currentRoute = event.url;
            }
        });
        this.translate.onLangChange.subscribe((event: LangChangeEvent) => {
            this.isRtl = event.lang == 'ur';
          });
    }
    
    private configureOAuth() {
        this.oauthService.configure(authConfig);
        this.oauthService.setupAutomaticSilentRefresh();
        this.oauthService.tokenValidationHandler = new JwksValidationHandler();
        this.oauthService.loadDiscoveryDocumentAndTryLogin();
        // this.oauthService.silentRefresh()
        // .then(info => console.debug('refresh ok', info))
        // .catch(err => console.error('refresh error', err));
    }

    private setLanguages(){
        this.translate.addLangs(["en", "ur"]);
        this.translate.setDefaultLang('en');

        this.translate.get('APP.TITLE').subscribe((res: string) => {
            this.titleService.setTitle(res);
        });

        let browserLang: string = this.translate.getBrowserLang();
        var selectedLang = localStorage.getItem('ui-lang'); 

        this.translate.use(selectedLang ? selectedLang : (browserLang.match(/en|ur/) ? browserLang : 'en'));
        this.isRtl = selectedLang == 'ur';        
    }
 }