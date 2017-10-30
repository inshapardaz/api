import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { FormsModule , FormBuilder } from '@angular/forms';

import { DictionaryService } from '../../../services/dictionary.service';
import { Word } from '../../../models/Word';
import { AlertService } from '../../../services/alert.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'word',
    templateUrl: './word.component.html'
})

export class WordComponent {
    private sub: Subscription;
    isBusy : boolean = false;
    showEditDialog : boolean = false;
    errorMessage: string;
    dictionaryId : number;
    wordId : number;
    word : Word;
    
    constructor(private route: ActivatedRoute,
        private router: Router,
        private alertService: AlertService,
        private translate: TranslateService,
        private dictionaryService: DictionaryService){
    }
    ngOnInit() {
        this.sub = this.route.params.subscribe(params => {
            this.dictionaryId = params['id'];
            this.wordId = params['wordId'];
            this.getWord();
        });
    }

    ngOnDestroy() {
        this.sub.unsubscribe();
    }

    getWord() {
        this.isBusy = true;
        this.dictionaryService.getWordById(this.dictionaryId, this.wordId)
            .subscribe(
            word => {
                this.word = word;
                this.isBusy = false;
            },
            error => {
                this.isBusy = false;
                this.alertService.error(this.translate.instant('WORD.MESSAGES.LOAD_FAILURE'));
                this.errorMessage = <any>error;
            });
    }

    editWord() {
        this.showEditDialog = true;
    }

    deleteWord(){
        this.isBusy = true;
        this.dictionaryService.deleteWord(this.word.deleteLink)
        .subscribe(r => {
            this.isBusy = false;
            this.alertService.success(this.translate.instant('WORD.MESSAGES.DELETE_SUCCESS', { title : this.word.title }));            
            this.router.navigate(['dictionaryLink', this.word.dictionaryLink ])
        }, error =>{
            this.errorMessage = <any>error;
            this.isBusy = false;
            this.alertService.error(this.translate.instant('WORD.MESSAGES.DELETE_FAILURE', { title : this.word.title}));            
        });
    }

    onEditClosed(created : boolean){
        this.showEditDialog = false;
        if (created){
            this.getWord();
        }
    }
}
