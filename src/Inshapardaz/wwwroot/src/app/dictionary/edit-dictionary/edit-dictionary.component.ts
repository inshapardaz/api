import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';

import { DictionaryService } from '../../../services/dictionary.service';
import { Dictionary } from '../../../models/Dictionary';
import { Languages } from '../../../models/language';

@Component({
    selector: 'edit-dictionaries',
    templateUrl: './edit-dictionary.html'
})
export class EditDictionaryComponent {
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
        console.log('dialog - visible ' + isVisible);
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
    
    constructor(private dictionaryService: DictionaryService, 
                private router: Router) {
        this.languages = Object.keys(this.languagesEnum).filter(Number);
        this.model.language = Languages.Urdu;
    }  

    onSubmit(){
        console.log(this.createLink);
        this.isBusy = false;
        if (this.isCreating){
            this.dictionaryService.createDictionary(this.createLink, this.model)
            .subscribe(m => {
                this.isBusy = false;
                this.onClosed.emit(true);
                this.visible = false;
            },
            this.handlerCreationError);    
        } else {
            this.dictionaryService.updateDictionary(this.model.updateLink, this.model)
            .subscribe(m => {
                this.isBusy = false;
                this.onClosed.emit(true);
                this.visible = false;
            },
            this.handlerCreationError);
        }
    }

    onClose(){
        this.onClosed.emit(false);
        this.visible = false;
    }

    handlerError(error : any) {
        this.isBusy = false;
    }

    handlerCreationError(error : any) {
        
    }
}