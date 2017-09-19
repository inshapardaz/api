import { Component } from '@angular/core';
import { Router, ActivatedRoute, NavigationExtras } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { FormsModule , FormBuilder } from '@angular/forms';
import { Observable }  from 'rxjs/Observable';
import { TranslateService } from '@ngx-translate/core';

import { Languages } from '../../../models/language';
import { DictionaryService } from '../../../services/dictionary.service';
import { Dictionary } from '../../../models/dictionary';
import { Word } from '../../../models/word';
import { WordPage } from '../../../models/WordPage';
import { DictionaryIndex } from '../../../models/Dictionary';
import { AlertService } from '../../../services/alert.service';

export class Index{
    title : string;
    link : string;
}

@Component({
    selector: 'dictionary',
    templateUrl: '../empty.html'
})

export class DictionaryByLinkComponent {
    private sub: Subscription;
    
    constructor(private route: ActivatedRoute,
        private router: Router){
    }

    ngOnInit(){
        this.sub = this.route.params.subscribe(params => {
            let dictionaryLink = params['link'];
            if (dictionaryLink === '' || dictionaryLink == null){
                this.router.navigate(['dictionaries']);    
            } else {
                let id  = dictionaryLink.substring( dictionaryLink.lastIndexOf('/') + 1)
                this.router.navigate(['dictionary', id]);
            }
        });
    }

    ngOnDestroy() {
        this.sub.unsubscribe();
      }
}


@Component({
    selector: 'dictionary',
    templateUrl: './dictionary.component.html'
})

export class DictionaryComponent {
    private sub: Subscription;
    isInSearch : boolean = false;
    isLoading : boolean = false;
    errorMessage: string;
    id : number;
    dictionary : Dictionary;
    searchText : Observable<string>;
    selectedIndex : Observable<string>;
    index : DictionaryIndex;
    wordPage : WordPage;
    loadedLink : string;
    indexes : Array<string>;
    pageNumber : number = 0;

    selectedWord : Word = null;
    createWordLink : string;
    showCreateDialog : boolean = false;

    Languages = Languages;
    

    public searchForm = this.fb.group({
        query: [""]
    });

    constructor(private route: ActivatedRoute,
        private router: Router,
        public fb: FormBuilder,
        private alertService: AlertService,
        private translate: TranslateService,
        private dictionaryService: DictionaryService){
    }

    ngOnInit() {
        this.sub = this.route.params.subscribe(params => {
            this.id = params['id'];
            this.pageNumber = params['page'] || 1;
            if (this.dictionary == null){
                this.getDictionary(d => {
                    this.getWords(d.indexLink);
                });
            } else{
                this.getWords(this.dictionary.indexLink);
            }
        });

        //Subscribe to search query parameter
        this.searchText = this.route.queryParams
            .map(params => params['search'] || '');
        this.searchText.subscribe(
            (val) => {
                if (val != null && val !== "") {
                    this.searchForm.controls.query.setValue(val);
                    if (this.dictionary == null){
                        this.getDictionary(d => {
                            this.doSearch();
                        });
                    } else {
                        this.doSearch();
                    }
                }
            });

        // Subscribe to startWith query parameter
        this.selectedIndex = this.route.queryParams
            .map(params => params['startWith'] || '');
        this.selectedIndex.subscribe(
            (val) => {
                if (val !== "") this.getIndex(val);
            });
        
    }

    ngOnDestroy() {
        this.sub.unsubscribe();
    }
      

    getIndex(index : string){
        this.isInSearch = false;
        this.isLoading = true;
        this.dictionaryService.getWordStartingWith(index, this.pageNumber)
            .subscribe(
                words => {
                    this.wordPage = words;
                    this.isLoading = false;
                },
                error => {
                    this.isLoading = false;
                    this.alertService.error(this.translate.instant('DICTIONARY.MESSAGES.LOADING_FAILURE'));
                    this.errorMessage = <any>error;
            });
    }

    getDictionary(callback) {
        this.isLoading = true;
        this.dictionaryService.getDictionary(this.id)
            .subscribe(
            dict => { 
                this.dictionary = dict;
                this.isLoading = false;
                this.createWordLink = dict.createWordLink;
                callback(dict);
            },
            error => {
                this.isLoading = false;
                this.alertService.error(this.translate.instant('DICTIONARY.MESSAGES.LOADING_FAILURE'));
                this.errorMessage = <any>error;
            });
    }
    
    getWords(link){
        this.isInSearch = false;
        this.isLoading = true;
        this.dictionaryService.getWords(link, this.pageNumber)
            .subscribe(
                words => {
                    this.wordPage = words;
                    this.isLoading = false;
                    this.loadedLink = link;
                },
                error => {
                    this.isLoading = false;
                    this.alertService.error(this.translate.instant('DICTIONARY.MESSAGES.WORDS_LOADING_FAILURE'));
                    this.errorMessage = <any>error;
            });
    }

    goNext(){
        this.pageNumber++
        this.navigateToPage();
    }
    goPrevious(){
        this.pageNumber--;
        this.navigateToPage();
    }

    gotoIndex(index:DictionaryIndex){
        let navigationExtras: NavigationExtras = {
            queryParams: { 'startWith': index.link },
        };
        this.router.navigate(['/dictionary', this.id], navigationExtras);
        
    }

    gotoSearch(){
        let query = this.searchForm.controls.query.value;
        
        if (query == null || query.length < 0) return;

        let navigationExtras: NavigationExtras = {
            queryParams: { 'search': query }
        };
        this.router.navigate(['/dictionary', this.id], navigationExtras);
    }

    navigateToPage(){
        var startWith = this.route.snapshot.queryParams["startWith"];
        var search = this.route.snapshot.queryParams["search"];
        if (startWith != null && startWith != ''){
            let navigationExtras: NavigationExtras = {
                queryParams: { 'startWith': startWith }
            };
            this.router.navigate(['dictionary', this.id, this.pageNumber ], navigationExtras);
        } else if (search != null && search != ''){
            let navigationExtras: NavigationExtras = {
                queryParams: { 'search': search }
            };
            this.router.navigate(['dictionary', this.id, this.pageNumber ], navigationExtras);            
        } else {
            this.router.navigate(['dictionary', this.id, this.pageNumber ]);
        }
    }

    reloadPage(){    
        this.getWords(this.loadedLink);
    }
    
    clearSearch(){
        this.searchForm.setValue({ query : ''});
        this.isInSearch = false;
        this.reloadPage();
    }

    doSearch() {
        var searchValue = this.searchForm.controls.query.value;
        if(searchValue != null && searchValue.length > 0){
              this.isInSearch = true;
              this.isLoading = true; 
              this.dictionaryService.searchWords(this.dictionary.searchLink, searchValue, this.pageNumber)
                    .subscribe( words => {
                        this.wordPage = words;
                        this.isLoading = false;
                },
                error => {
                    this.isLoading = false;
                    this.alertService.error(this.translate.instant('DICTIONARY.MESSAGES.SEARCH_LOADING_FAILURE'));
                    this.errorMessage = <any>error;
            });
        }
    }

    addWord(){
        this.showCreateDialog = true;
    }
    onCreateClosed(created : boolean){
        this.showCreateDialog = false;
        this.selectedWord = null;
        if (created){
            this.reloadPage();
        }
    }

    editWord(word : Word){
        this.showCreateDialog = true;
        this.selectedWord = word;
    }

    deleteWord(word : Word){
        this.isLoading = true;
        this.dictionaryService.deleteWord(word.deleteLink)
        .subscribe(r => {
            this.alertService.success(this.translate.instant('WORD.MESSAGES.DELETE_SUCCESS', { title : word.title }));            
            this.reloadPage();
        }, e => {
            this.errorMessage = <any>e;
            this.isLoading = false;
            this.alertService.error(this.translate.instant('WORD.MESSAGES.DELETE_FAILURE', { title : word.title}));            
        });
    }
}
