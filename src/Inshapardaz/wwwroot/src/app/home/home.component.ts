import { Component } from '@angular/core';
import {TranslateService} from 'ng2-translate/ng2-translate';

@Component({
    selector: 'my-app',
    templateUrl: './home.component.html'
})

export class HomeComponent {
    constructor(public translate: TranslateService) {
    }
 }
