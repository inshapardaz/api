import { Component } from '@angular/core'
import { Router, ActivatedRoute  } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';

import { DictionaryService } from '../../services/dictionary.service';
import { Word } from '../../models/word';
import { Page } from '../../models/page';

@Component({
    selector: 'dictionary-wordlist',
    templateUrl: 'app/components/dictionary/wordList.html'
})

export class WordListComponent {
    private sub: Subscription;
    startWith: string;
    pageNumber: number = 1;
    pageSize: number = 12;
    pageCount: number = 0;    
    words : Word[];
    errorMessage: string;
    isLoading : boolean = false;

    pages : Page[];
    
    constructor(private route: ActivatedRoute,
        private router: Router,
        private dictionaryService: DictionaryService) {
    }
    ngOnInit() {
        this.sub = this.route.params.subscribe(params => {
            this.startWith = params['startWith'];
            if (params['pageNumber'])
                this.pageNumber = params['pageNumber'];
            this.getWords();
        });
    }

    ngOnDestroy() {
        this.sub.unsubscribe();
    }

    getWords(){
        this.errorMessage = null;
        this.isLoading = true;

        this.dictionaryService.wordStartingWith(this.startWith, this.pageNumber, this.pageSize)
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