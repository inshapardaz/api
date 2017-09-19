import { TranslateService } from '@ngx-translate/core';
import { AlertService } from '../../../services/alert.service';
import { Component, Input, transition } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { DictionaryService } from '../../../services/dictionary.service';
import { Translation } from '../../../models/translation';
import {RelationTypes} from '../../../models/relationTypes';

@Component({
    selector: 'word-translations',
    templateUrl: './Translations.html'
})

export class WordTranslationsComponent {
    public _translationsLink: string;
    public relationTypes : RelationTypes;
    public isLoading : boolean = false;
    public errorMessage: string;
    public translations : Array<Translation>;

    selectedTranslation : Translation;
    showEditDialog : boolean = false;

    @Input() createLink : string;
    @Input() wordDetailId : string;  
    @Input()
    set translationsLink(translationLink: string) {
        this._translationsLink = (translationLink) || '';
        this.getTranslations();
    }
    get translationsLink(): string { return this._translationsLink; }

    constructor(private route: ActivatedRoute,
        private router: Router,
        private alertService: AlertService,
        private translate: TranslateService,
        private dictionaryService: DictionaryService){
    }

    getTranslations() {
        this.isLoading = true;
        this.dictionaryService.getWordTranslations(this._translationsLink)
            .subscribe(
            translations => { 
                this.translations = translations;
                this.isLoading = false;
            },
            error => {
                this.errorMessage = <any>error;
                this.isLoading = false;
                this.alertService.error(this.translate.instant('WORDTRANSLATION.MESSAGES.LOAD_FAILURE'));                
            });
    }

    addTranslation(){
        this.selectedTranslation = null;
        this.showEditDialog = true;
    }

    editTranslation(translation : Translation){
        this.selectedTranslation = translation;
        this.showEditDialog = true;
    }

    deleteTranslation(translation : Translation){
        this.dictionaryService.deleteWordTranslation(translation.deleteLink)
        .subscribe(r => {
            this.alertService.success(this.translate.instant('WORDTRANSLATION.MESSAGES.DELETE_SUCCESS'));            
            this.getTranslations();
        }, error => {
            this.errorMessage = <any>error;
            this.isLoading = false;
            this.alertService.error(this.translate.instant('WORDTRANSLATION.MESSAGES.DELETE_FAILURE'));            
        });
    }
    
    onEditClosed(created : boolean){
        this.showEditDialog = false;
        if (created){
            this.getTranslations();
        }
    }
}