import {Component} from '@angular/core';
import { Router } from '@angular/router';

declare var App : any;

@Component({
    selector: 'home',
    templateUrl: 'app/components/home/home.html',
})

export class HomeComponent {
    searchText: string = "";

    constructor(
        private router: Router) { }
    onSearch(): void {
        this.router.navigate(['/search', this.searchText]);
    }

    ngAfterViewInit() {
        App.initHelper('appear');
    }
}