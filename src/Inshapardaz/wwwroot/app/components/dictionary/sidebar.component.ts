import { Component } from '@angular/core';

import { DictionaryService } from '../../services/dictionary.service';
import { DictionaryIndex } from '../../models/dictionaryIndex';
import { Link } from '../../models/link';

@Component({
    selector: 'sidebar',
    templateUrl: 'app/components/dictionary/sidebar.html',
})

export class DictionarySidebarComponent {
    public dictionaryIndexes : Link[]; 
    public errorMessage : any;

    constructor(private dictionaryService: DictionaryService) {

    }

    ngOnInit() { this.getIndex(); }

    getIndex() {
        this.dictionaryService.getIndex()
            .subscribe(
            index => this.dictionaryIndexes = index.indexes,
            error => this.errorMessage = <any>error);
    }

    toggleSidebar(){
        var $windowW = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
        var $lPage = jQuery('#page-container');

        if ($windowW > 991) {
            $lPage.toggleClass('sidebar-o');
        } else {
            $lPage.toggleClass('sidebar-o-xs');
        }
    }
}