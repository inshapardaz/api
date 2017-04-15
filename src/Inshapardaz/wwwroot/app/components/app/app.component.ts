/// <reference path="../../../typings/index.d.ts" />

import {Component} from '@angular/core';
import { RouterModule , Router, NavigationStart } from '@angular/router';

import {Auth} from '../../services/auth.service';

declare var App : any;

@Component({
    selector: 'my-app',
    templateUrl: 'app/components/app/app.html'
})

export class AppComponent {
    isHome :boolean = false;
    currentRoute: string = "";
    constructor(private router: Router, private auth: Auth) {
        this.router.events
        .subscribe(event => {
            if (event instanceof NavigationStart){
                 this.currentRoute = event.url;
                 this.isHome = event.url === '/home' || event.url === '/404' || event.url === '/';
            }
        });
    }

    ngOnInit(){
        App.init();
    }
} 