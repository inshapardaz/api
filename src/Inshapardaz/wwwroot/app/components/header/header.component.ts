import { Component, Input } from '@angular/core';
import { Router, RouterModule  } from '@angular/router';

import {Auth} from '../../services/auth.service';

@Component({
    selector: 'header',
    templateUrl: 'app/components/header/header.html'
})

export class HeaderComponent {
    @Input() miniHeader: boolean = false;
    searchText: string = "";

    constructor(
        private router: Router,
        private auth: Auth
    ) {

     }

    onSearch(event): void {
        if (event.keyCode == 13) {
            this.router.navigate(['/search', this.searchText]);
        }
    }

    toggleSidebar(){
        var $lPage = jQuery('#page-container');
        var $windowW = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;

        if ($windowW > 991) {
            $lPage.toggleClass('sidebar-o');
        } else {
            $lPage.toggleClass('sidebar-o-xs');
        }
    }
}