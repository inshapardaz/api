import { Component } from '@angular/core';
import {TranslateService} from '@ngx-translate/core';

@Component({
    selector: 'my-app',
    templateUrl: './home.component.html'
})

export class HomeComponent {
    constructor(public translate: TranslateService) {
    }
 }
