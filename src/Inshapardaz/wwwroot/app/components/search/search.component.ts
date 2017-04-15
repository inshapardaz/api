import { Component } from '@angular/core';
import { Router, ActivatedRoute  } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';

import { DictionaryService } from '../../services/dictionary.service';

import { Word } from '../../models/word';
import { Page } from '../../models/page';

@Component({
    selector: 'search',
    templateUrl: 'app/components/search/search.html',
})

export class SearchComponent {
    private sub: Subscription;
    searchText: string;

    words : Word[];

    pageNumber: number = 1;
    pageSize: number = 10;
    pageCount: number = 0;    

    errorMessage: string;
    isLoading : boolean = false;
    pages : Page[];

    constructor(private route: ActivatedRoute, 
                private router: Router,
                private dictionaryService: DictionaryService) {
    }

    ngOnInit() {
        this.sub = this.route.params.subscribe(params => {
            this.searchText = params['searchText'];
            if (params['pageNumber'])
                this.pageNumber = params['pageNumber'];
            this.getSearchResult();
        });
    }

    ngOnDestroy() {
        this.sub.unsubscribe();
    }

    onSearch() {
        this.router.navigateByUrl('/search/' + this.searchText);
    }

    getSearchResult() {
        this.errorMessage = null;
        this.isLoading = true;

        this.dictionaryService.searchWord(this.searchText, this.pageNumber, this.pageSize)
        .subscribe(
            wordPage => {
                this.words = wordPage.words;
                this.pageNumber = wordPage.currentPageIndex;
                this.pageCount = wordPage.pageCount;

                this.pages = [];
                this.pages.push(new Page(wordPage.currentPageIndex));

                let count = 5;
                for (var i=1; i < count; i++){
                    if (wordPage.currentPageIndex + i < wordPage.pageCount)
                        this.pages.push(new Page(wordPage.currentPageIndex + i));    
                }

                this.isLoading = false;
            },
            error => {
                this.errorMessage = <any>error;
                this.isLoading = false;
            });
    }
}