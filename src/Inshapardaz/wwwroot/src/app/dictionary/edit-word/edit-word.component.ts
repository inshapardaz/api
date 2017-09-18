import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';

import {TranslateService} from '@ngx-translate/core';

import { DictionaryService } from '../../../services/dictionary.service';
import { Word } from '../../../models/Word';
import { Languages } from '../../../models/language';

@Component({
    selector: 'edit-word',
    templateUrl: './edit-word.html'
})
export class EditWordComponent {
    model = new Word();
    languages : any[];
    languagesEnum = Languages;
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
                private translate: TranslateService) {
        this.languages = Object.keys(this.languagesEnum).filter(Number);
    }  

    onSubmit(){
        this.isBusy = false;
        if (this.isCreating){
            this.dictionaryService.createWord(this.createLink, this.model)
            .subscribe(m => {
                this.isBusy = false;
                this.onClosed.emit(true);
                this.visible = false;
            },
            this.handlerCreationError);    
        } else {
            this.dictionaryService.updateWord(this.model.updateLink, this.model)
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