import { TranslateService } from '@ngx-translate/core';
import { AlertService } from '../../../services/alert.service';
import { Component, Input  } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { DictionaryService } from '../../../services/dictionary.service';
import { Meaning } from '../../../models/meaning';
import {RelationTypes} from '../../../models/relationTypes';

@Component({
    selector: 'word-meanings',
    templateUrl: './Meanings.html'
})

export class MeaningsComponent {
    public _meaningsLink: string;
    public relationTypes : RelationTypes;
    public isLoading : boolean = false;
    public errorMessage: string;
    public meanings : Array<Meaning>;

    showEditDialog : boolean = false;
    selectedMeaning : Meaning = null;
    @Input() createLink : string;
    @Input() wordId : number;
    @Input()
    set meaningsLink(relationsLink: string) {
        this._meaningsLink = (relationsLink) || '';
        this.getMeanings();
    }
    get meaningsLink(): string { return this._meaningsLink; }

    constructor(private route: ActivatedRoute,
        private router: Router,
        private alertService: AlertService,
        private translate: TranslateService,
        private dictionaryService: DictionaryService){
    }

    getMeanings() {
        this.isLoading = true;
        this.dictionaryService.getMeanings(this._meaningsLink)
            .subscribe(
            meanings => { 
                this.meanings = meanings;
                this.isLoading = false;
            },
            error => {
                this.alertService.error(this.translate.instant('MEANING.MESSAGES.LOAD_FAILURE'));                
                this.errorMessage = <any>error;
            });
    }
    
    addMeaning(){
        this.selectedMeaning = null;
        this.showEditDialog = true;
    }

    editMeaning(meaning : Meaning) {
        this.selectedMeaning = meaning;
        this.showEditDialog = true;
    }

    deleteMeaning(meaning : Meaning) {
        this.dictionaryService.deleteMeaning(meaning.deleteLink)
        .subscribe(r => {
            this.alertService.success(this.translate.instant('MEANING.MESSAGES.DELETE_SUCCESS'));            
            this.getMeanings();
        }, error => {
            this.errorMessage = <any>error;
            this.isLoading = false;
            this.alertService.error(this.translate.instant('MEANING.MESSAGES.DELETE_FAILURE'));            
        });
    }

    onEditClosed(created : boolean){
        this.showEditDialog = false;
        if (created){
            this.getMeanings();
        }
    }
}