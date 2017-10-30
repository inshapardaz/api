import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';

import {TranslateService} from '@ngx-translate/core';

import { DictionaryService } from '../../../services/dictionary.service';
import { Word } from '../../../models/Word';
import { Languages } from '../../../models/language';
import { GrammaticTypes } from '../../../models/grammaticalTypes';
import { AlertService } from '../../../services/alert.service';

@Component({
    selector: 'edit-word',
    templateUrl: './edit-word.html'
})
export class EditWordComponent {
    model = new Word();
    languages : any[];
    languagesEnum = Languages;
    attributesValues : any[];
    attributeEnum = GrammaticTypes;

    _visible : boolean = false;
    isBusy : boolean = false;
    isCreating : boolean = false;

    @Input() createLink:string = '';
    @Input() modalId:string = '';
    @Input() word:Word = null;
    @Output() onClosed = new EventEmitter<boolean>();

    @Input()
    set visible(isVisible: boolean) {
        this._visible = isVisible;
        this.isBusy = false;
        if (isVisible){
            if (this.word == null) {
                this.model = new Word();
                this.isCreating = true;
            } else {
                this.model = Object.assign({}, this.word);
                this.isCreating = false;
            }
            $('#'+ this.modalId).modal('show');
        } else {
            $('#'+ this.modalId).modal('hide');
        }
    }
     
    get visible(): boolean { return this._visible; }
    
    constructor(private dictionaryService: DictionaryService, 
                private router: Router,
                private translate: TranslateService,
                private alertService: AlertService) {
        this.languages = Object.keys(this.languagesEnum).filter(Number);
        this.attributesValues = Object.keys(this.attributeEnum).filter(Number)
    }  

    onSubmit(){
        this.isBusy = true;
        if (this.isCreating){
            this.dictionaryService.createWord(this.createLink, this.model)
            .subscribe(m => {
                this.isBusy = false;
                this.onClosed.emit(true);
                this.visible = false;
                this.alertService.success(this.translate.instant('WORD.MESSAGES.CREATION_SUCCESS', { title : this.model.title }));                
            }, e => {
                this.isBusy = false;
                this.alertService.error(this.translate.instant('WORD.MESSAGES.CREATION_FAILURE', { title : this.model.title }));
                
            });
        } else {
            this.dictionaryService.updateWord(this.model.updateLink, this.model)
            .subscribe(m => {
                this.isBusy = false;
                this.onClosed.emit(true);
                this.visible = false;
                this.alertService.success(this.translate.instant('WORD.MESSAGES.UPDATE_SUCCESS', { title : this.model.title }));
            }, e => {
                this.isBusy = false;
                this.alertService.error(this.translate.instant('WORD.MESSAGES.UPDATE_FAILURE', { title : this.model.title }));                
            });
        }
    }

    onClose(){
        this.onClosed.emit(false);
        this.visible = false;
    }
}