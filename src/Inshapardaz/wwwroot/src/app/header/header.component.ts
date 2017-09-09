import { Component, Input } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import {TranslateService} from 'ng2-translate/ng2-translate';

import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'header',
    templateUrl: './header.component.html'})

export class HeaderComponent {
    @Input() miniHeader: boolean = false;
    searchText: string = "";
    profile : any;
    
    constructor(
        private router: Router,
        private auth: AuthService,
        public translate: TranslateService
    ) {
    }

    ngOnInit() {
        if (this.auth.currentUser) {
            this.profile = this.auth.currentUser;
        } else {
            this.profile = this.auth.getUser();
        }
    }
    onSearch(event: any): void {
        if (event.keyCode == 13) {
            this.router.navigate(['/search', this.searchText]);
        }
    }
}
