import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';

import { DictionaryService } from '../../../services/dictionary.service';
import { Dictionary } from '../../../models/Dictionary';
import { Languages } from '../../../models/language';

@Component({
    selector: 'create-dictionaries',
    templateUrl: './create-dictionary.component.html'
})
export class CreateDictionaryComponent {
    model = new Dictionary();
    languages : any[];
    languagesEnum = Languages;
    _visible : boolean = false;

    @Input() createLink:string = '';
    @Input() modalId:string = '';
    @Output() onClosed = new EventEmitter<boolean>();

    @Input()
    set visible(isVisible: boolean) {
        console.log('dialog - visible ' + isVisible);
        this._visible = isVisible;
        if (isVisible){
            this.model = new Dictionary();
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
        this.dictionaryService.createDictionary(this.createLink, this.model)
        .subscribe(m => {
            this.onClosed.emit(true);
            this.visible = false;
        },
        this.handlerCreationError);
    }

    onClose(){
        this.onClosed.emit(false);
        this.visible = false;
    }

    handlerError(error : any) {
        
    }

    handlerCreationError(error : any) {
        
    }
}