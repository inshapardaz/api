import { AlertService } from '../../../services/alert.service';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';

import {TranslateService} from '@ngx-translate/core';

import { DictionaryService } from '../../../services/dictionary.service';
import { Meaning } from '../../../models/Meaning';
import { Languages } from '../../../models/language';

@Component({
    selector: 'edit-meaning',
    templateUrl: './edit-meaning.html'
})
export class EditMeaningComponent {
    model = new Meaning();
    languages : any[];
    languagesEnum = Languages;

    _visible : boolean = false;
    isBusy : boolean = false;
    isCreating : boolean = false;

    @Input() createLink:string = '';
    @Input() modalId:string = '';
    @Input() meaning:Meaning = null;
    @Output() onClosed = new EventEmitter<boolean>();

    @Input()
    set visible(isVisible: boolean) {
        this._visible = isVisible;
        this.isBusy = false;
        if (isVisible){
            if (this.meaning == null) {
                this.model = new Meaning();
                this.isCreating = true;
            } else {
                this.model = Object.assign({}, this.meaning);
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
            this.dictionaryService.createMeaning(this.createLink, this.model)
            .subscribe(m => {
                this.isBusy = false;
                this.onClosed.emit(true);
                this.visible = false;
                this.alertService.success(this.translate.instant('MEANING.MESSAGES.CREATION_SUCCESS'));                
            }, error => {
                this.isBusy = false;        
                this.alertService.error(this.translate.instant('MEANING.MESSAGES.CREATION_FAILURE'));                
            });    
        } else {
            this.dictionaryService.updateMeaning(this.model.updateLink, this.model)
            .subscribe(m => {
                this.isBusy = false;
                this.onClosed.emit(true);
                this.visible = false;
                this.alertService.success(this.translate.instant('MEANING.MESSAGES.UPDATE_SUCCESS'));                
            },error => {
                this.isBusy = false;        
                this.alertService.error(this.translate.instant('MEANING.MESSAGES.UPDATE_FAILURE'));                
            });
        }
    }

    onClose(){
        this.visible = false;        
        this.onClosed.emit(false);
    }
}