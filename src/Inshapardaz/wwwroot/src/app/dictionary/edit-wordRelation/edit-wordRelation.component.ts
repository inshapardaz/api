import { Observable } from 'rxjs/Rx';
import { DomSanitizer, SafeHtml } from "@angular/platform-browser";
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';

import {TranslateService} from 'ng2-translate';

import { DictionaryService } from '../../../services/dictionary.service';
import { Relation } from '../../../models/relation';
import { Languages } from '../../../models/language';
import { WordPage } from '../../../models/WordPage';
import { Word } from '../../../models/Word';
import { RelationTypes } from '../../../models/relationTypes';

@Component({
    selector: 'edit-wordRelation',
    templateUrl: './edit-wordRelation.html'
})
export class EditWordRelationComponent {
    model = new Relation();
    languages : any[];
    languagesEnum = Languages;

    relationTypesValues : any[];
    relationTypesEnum = RelationTypes;

    _visible : boolean = false;
    isBusy : boolean = false;
    isCreating : boolean = false;

    @Input() createLink:string = '';
    @Input() dictionaryLink:string = '';
    @Input() modalId:string = '';
    @Input() relation:Relation = null;
    @Input() sourceWord:Word = null;
    @Output() onClosed = new EventEmitter<boolean>();

    @Input()
    set visible(isVisible: boolean) {
        this._visible = isVisible;
        this.isBusy = false;
        if (isVisible){
            if (this.relation == null) {
                this.model = new Relation();
                this.model.sourceWordId = this.sourceWord.id;
                this.isCreating = true;
            } else {
                this.model = Object.assign({}, this.relation);
                this.model.sourceWordId = this.sourceWord.id;
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
                private _sanitizer: DomSanitizer) {
        this.languages = Object.keys(this.languagesEnum).filter(Number);
        this.relationTypesValues = Object.keys(this.relationTypesEnum).filter(Number)
    }  

    observableSource = (keyword: any): Observable<Word[]> => {
        if (keyword) {
          return this.dictionaryService.getWordsStartingWith(this.dictionaryLink, keyword);
        } else {
          return Observable.of([]);
        }
      }
      autocompleteListFormatter = (data: any) => {
        return this._sanitizer.bypassSecurityTrustHtml(data.title);
      }

      relatedWordChanged(e: Word): void {
        this.model.relatedWordId = e.id;
        this.model.relatedWord = e.title;
    }
    onSubmit(){
        this.isBusy = false;
        if (this.isCreating){
            this.model.sourceWord = this.sourceWord.title;
            this.dictionaryService.createRelation(this.createLink, this.model)
            .subscribe(m => {
                this.isBusy = false;
                this.onClosed.emit(true);
                this.visible = false;
            },
            this.handlerError);    
        } else {
            this.dictionaryService.updateRelation(this.model.updateLink, this.model)
            .subscribe(m => {
                this.isBusy = false;
                this.onClosed.emit(true);
                this.visible = false;
            },
            this.handlerError);
        }
    }

    onClose(){
        this.visible = false;        
        this.onClosed.emit(false);
    }

    handlerError(error : any) {
        this.isBusy = false;
    }
}