import * as v8 from 'v8';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';

import {TranslateService} from '@ngx-translate/core';

import { DictionaryService } from '../../../services/dictionary.service';
import { AlertService } from '../../../services/alert.service';

import { Dictionary } from '../../../models/Dictionary';
import { Languages } from '../../../models/language';

@Component({
    selector: 'edit-dictionaries',
    templateUrl: './edit-dictionary.html'
})
export class EditDictionaryComponent {
    constructor(private dictionaryService: DictionaryService, 
        private router: Router,
        private translate: TranslateService,
        private alertService: AlertService) {
        this.languages = Object.keys(this.languagesEnum).filter(Number);
        this.model.language = Languages.Urdu;
    } 

    model = new Dictionary();
    languages : any[];
    languagesEnum = Languages;
    _visible : boolean = false;
    isBusy : boolean = false;
    isCreating : boolean = false;

    @Input() createLink:string = '';
    @Input() modalId:string = '';
    @Input() dictionary:Dictionary = null;
    @Output() onClosed = new EventEmitter<boolean>();

    @Input()
    set visible(isVisible: boolean) {
        this._visible = isVisible;
        this.isBusy = false;
        if (isVisible){
            if (this.dictionary == null) {
                this.model = new Dictionary();
                this.isCreating = true;
            } else {
                this.model = Object.assign({}, this.dictionary);
                this.isCreating = false;
            }
            $('#'+ this.modalId).modal('show');
        } else {
            $('#'+ this.modalId).modal('hide');
        }
    }
     
    get visible(): boolean { return this._visible; }

    onSubmit(){
        if (this.isBusy){
            return;
        }
        
        this.isBusy = true;
        if (this.isCreating){
            this.dictionaryService.createDictionary(this.createLink, this.model)
            .subscribe(m => {
                this.isBusy = false;
                this.onClosed.emit(true);
                this.visible = false;
                this.alertService.success(this.translate.instant('DICTIONARIES.MESSAGES.CREATION_SUCCESS', { name : this.model.name }));                
            }, e => {
                this.isBusy = false;
                this.alertService.error(this.translate.instant('DICTIONARIES.MESSAGES.CREATION_FAILURE', { name : this.model.name }));
            });    
        } else {
            this.dictionaryService.updateDictionary(this.model.updateLink, this.model)
            .subscribe(m => {
                this.isBusy = false;
                this.onClosed.emit(true);
                this.visible = false;
                this.alertService.success(this.translate.instant('DICTIONARIES.MESSAGES.UPDATE_SUCCESS', { name : this.model.name }));
            }, e => {
                this.isBusy = false;
                this.alertService.error(this.translate.instant('DICTIONARIES.MESSAGES.UPDATE_FAILURE', { name : this.model.name }));
            });
        }
    }

    onClose(){
        this.onClosed.emit(false);
        this.visible = false;
    }
}