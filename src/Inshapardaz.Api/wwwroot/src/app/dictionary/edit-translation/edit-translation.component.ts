import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';

import {TranslateService} from '@ngx-translate/core';

import { DictionaryService } from '../../../services/dictionary.service';
import { Translation } from '../../../models/Translation';
import { Languages } from '../../../models/language';
import { GrammaticTypes } from '../../../models/grammaticalTypes';
import { AlertService } from '../../../services/alert.service';

@Component({
    selector: 'edit-translation',
    templateUrl: './edit-translation.html'
})
export class EditWordTranslationComponent {
    model = new Translation();
    languages : any[];
    languagesEnum = Languages;

    _visible : boolean = false;
    isBusy : boolean = false;
    isCreating : boolean = false;

    @Input() createLink:string = '';
    @Input() modalId:string = '';
    @Input() translation:Translation = null;
    @Output() onClosed = new EventEmitter<boolean>();

    @Input()
    set visible(isVisible: boolean) {
        this._visible = isVisible;
        this.isBusy = false;
        if (isVisible){
            if (this.translation == null) {
                this.model = new Translation();
                this.isCreating = true;
            } else {
                this.model = Object.assign({}, this.translation);
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
    }  

    onSubmit(){
        this.isBusy = false;
        if (this.isCreating){
            this.dictionaryService.createWordTranslation(this.createLink, this.model)
            .subscribe(m => {
                this.isBusy = false;
                this.onClosed.emit(true);
                this.visible = false;
                this.alertService.success(this.translate.instant('WORDTRANSLATION.MESSAGES.CREATION_SUCCESS'));                
            }, error => {
                this.isBusy = false;        
                this.alertService.error(this.translate.instant('WORDTRANSLATION.MESSAGES.CREATION_FAILURE'));                
            });    
        } else {
            this.dictionaryService.updateWordTranslation(this.model.updateLink, this.model)
            .subscribe(m => {
                this.isBusy = false;
                this.onClosed.emit(true);
                this.visible = false;
                this.alertService.success(this.translate.instant('WORDTRANSLATION.MESSAGES.UPDATE_SUCCESS'));                
            }, error => {
                this.isBusy = false;        
                this.alertService.error(this.translate.instant('WORDTRANSLATION.MESSAGES.UPDATE_FAILURE'));                
            });
        }
    }

    onClose(){
        this.onClosed.emit(false);
        this.visible = false;
    }
}