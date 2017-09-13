import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';

import {TranslateService} from 'ng2-translate';

import { DictionaryService } from '../../../services/dictionary.service';
import { Meaning } from '../../../models/meaning';
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
                private translate: TranslateService) {
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
            },
            this.handlerCreationError);    
        } else {
            this.dictionaryService.updateMeaning(this.model.updateLink, this.model)
            .subscribe(m => {
                this.isBusy = false;
                this.onClosed.emit(true);
                this.visible = false;
            },
            this.handlerCreationError);
        }
    }

    onClose(){
        this.visible = false;        
        this.onClosed.emit(false);
    }

    handlerError(error : any) {
        this.isBusy = false;
    }

    handlerCreationError(error : any) {
        
    }
}