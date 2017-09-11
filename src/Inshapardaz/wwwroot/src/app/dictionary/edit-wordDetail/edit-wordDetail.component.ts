import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';

import {TranslateService} from 'ng2-translate';

import { DictionaryService } from '../../../services/dictionary.service';
import { WordDetail } from '../../../models/WordDetail';
import { Languages } from '../../../models/language';
import { GrammaticTypes } from '../../../models/grammaticalTypes';

@Component({
    selector: 'edit-wordDetail',
    templateUrl: './edit-wordDetail.html'
})
export class EditWordDetailComponent {
    model = new WordDetail();
    languages : any[];
    languagesEnum = Languages;

    attributesValues : any[];
    attributeEnum = GrammaticTypes;

    _visible : boolean = false;
    isBusy : boolean = false;
    isCreating : boolean = false;

    @Input() createLink:string = '';
    @Input() modalId:string = '';
    @Input() wordDetail:WordDetail = null;
    @Output() onClosed = new EventEmitter<boolean>();

    @Input()
    set visible(isVisible: boolean) {
        this._visible = isVisible;
        this.isBusy = false;
        console.log(this.createLink);
        if (isVisible){
            if (this.wordDetail == null) {
                this.model = new WordDetail();
                this.isCreating = true;
            } else {
                this.model = Object.assign({}, this.wordDetail);
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
        this.attributesValues = Object.keys(this.attributeEnum).filter(Number)
    }  

    onSubmit(){
        this.isBusy = false;
        if (this.isCreating){
            this.dictionaryService.createWordDetail(this.createLink, this.model)
            .subscribe(m => {
                this.isBusy = false;
                this.onClosed.emit(true);
                this.visible = false;
            },
            this.handlerCreationError);    
        } else {
            this.dictionaryService.updateWordDetail(this.model.updateLink, this.model)
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