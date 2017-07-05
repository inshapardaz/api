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
        if (this.auth.userProfile) {
            this.profile = this.auth.userProfile;
        } else {
            this.auth.getProfile((err, profile) => {
                this.profile = profile;
            });
        }
    }
    onSearch(event: any): void {
        if (event.keyCode == 13) {
            this.router.navigate(['/search', this.searchText]);
        }
    }
}
