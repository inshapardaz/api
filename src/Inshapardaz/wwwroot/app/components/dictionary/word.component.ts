import {Component, Input} from '@angular/core'
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';

import { DictionaryService } from '../../services/dictionary.service';
import { Word } from  '../../models/word';
import { WordDetail } from  '../../models/worddetail';
import { Relation } from '../../models/relation';

@Component({
    selector: 'dictionary-word',
    templateUrl: 'app/components/dictionary/word.html'
})

export class WordComponent {
    private sub: Subscription;
    id: string;
    errorMessage: string;
    word: Word;
    relations : Relation[];
    details : WordDetail[];

    isLoadingWord  : boolean = false;
    isLoadingDetails  : boolean = false;
    isLoadingRelations  : boolean = false;

    constructor(private route: ActivatedRoute,
        private router: Router,
        private dictionaryService: DictionaryService) {
    }

    ngOnInit() {
        this.sub = this.route.params.subscribe(params => {
            this.id = params['id'];
            this.getWord();
        });
    }

    ngOnDestroy() {
        this.sub.unsubscribe();
    }

    getWord() {
        this.isLoadingWord = true;
        this.dictionaryService.getWordByid(this.id)
            .subscribe(
            word => { 
                this.word = word;
                this.getWordDetails();
                this.getWordRelations();
                this.isLoadingWord = false;
            },
            error => {
                this.errorMessage = <any>error;
                this.isLoadingWord = false;                
            });
    }

    getWordDetails(){
        this.isLoadingDetails = true;
        this.dictionaryService.getWordDetails(this.word.detailsLink)
            .subscribe(
            details => {
                this.details = details;
                this.isLoadingDetails = false;
            },
            error => {
                this.errorMessage = <any>error;
                this.isLoadingDetails = false;                
            });
    }

    getWordRelations(){
        this.isLoadingRelations = true;        
        this.dictionaryService.getWordRelations(this.word.relationsLink)
            .subscribe(
            relations => {
                this.relations = relations;
                this.isLoadingRelations = false;
            },
            error => {
                this.errorMessage = <any>error;
                this.isLoadingRelations = false;                
            });
    }
}