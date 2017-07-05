import { Component } from '@angular/core';
import { ViewEncapsulation } from '@angular/core';
import { RouterModule , Router, NavigationStart } from '@angular/router';
import { Title }     from '@angular/platform-browser';

import {TranslateService} from 'ng2-translate/ng2-translate';

import {AuthService} from '../services/auth.service';

@Component({
    selector: 'my-app',
    templateUrl: './app.component.html',
    encapsulation: ViewEncapsulation.None
})

export class AppComponent {
    currentRoute: string = "";
    constructor(private router: Router, 
                private auth: AuthService, 
                private translate: TranslateService, 
                private titleService: Title) {
        this.setLanguages();
        this.router.events
        .subscribe(event => {
            if (event instanceof NavigationStart){
                 this.currentRoute = event.url;
            }
        });
        auth.handleAuthentication();
        auth.scheduleRenewal();
    }

    private setLanguages(){
        this.translate.addLangs(["en", "ur"]);
        this.translate.setDefaultLang('en');

        console.log(this.translate.get('APP.TITLE'));
        this.translate.get('APP.TITLE').subscribe((res: string) => {
            console.log('title changed to :' + res);
            this.titleService.setTitle(res);
        });

        let browserLang: string = this.translate.getBrowserLang();
        var selectedLang = localStorage.getItem('ui-lang'); 

        this.translate.use(selectedLang ? selectedLang : (browserLang.match(/en|ur/) ? browserLang : 'en'));
    }
 }