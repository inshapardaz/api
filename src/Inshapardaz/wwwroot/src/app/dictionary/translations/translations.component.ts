import { Component, Input  } from '@angular/core';
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
    @Input() createLink : string;
    @Input() wordDetailId : string;  
    @Input()
    set translationsLink(translationLink: string) {
        this._translationsLink = (translationLink) || '';
        this.getRelations();
    }
    get translationsLink(): string { return this._translationsLink; }

    constructor(private route: ActivatedRoute,
        private router: Router,
        private dictionaryService: DictionaryService){
    }

    getRelations() {
        this.isLoading = true;
        this.dictionaryService.getWordTranslations(this._translationsLink)
            .subscribe(
            translations => { 
                this.translations = translations;
                this.isLoading = false;
            },
            error => {
                this.errorMessage = <any>error;
            });
    }
}