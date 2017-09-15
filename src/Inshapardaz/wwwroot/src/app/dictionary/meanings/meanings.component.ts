import { Component, Input  } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { DictionaryService } from '../../../services/dictionary.service';
import { Meaning } from '../../../models/meaning';
import {RelationTypes} from '../../../models/relationTypes';

@Component({
    selector: 'word-meanings',
    templateUrl: './Meanings.html'
})

export class MeaningsComponent {
    public _meaningsLink: string;
    public relationTypes : RelationTypes;
    public isLoading : boolean = false;
    public errorMessage: string;
    public meanings : Array<Meaning>;

    showEditDialog : boolean = false;
    selectedMeaning : Meaning = null;
    @Input() createLink : string;
    @Input() wordDetailId : number;
    @Input()
    set meaningsLink(relationsLink: string) {
        this._meaningsLink = (relationsLink) || '';
        this.getMeanings();
    }
    get meaningsLink(): string { return this._meaningsLink; }

    constructor(private route: ActivatedRoute,
        private router: Router,
        private dictionaryService: DictionaryService){
    }

    getMeanings() {
        this.isLoading = true;
        this.dictionaryService.getMeanings(this._meaningsLink)
            .subscribe(
            meanings => { 
                this.meanings = meanings;
                this.isLoading = false;
            },
            error => {
                this.errorMessage = <any>error;
            });
    }
    addMeaning(){
        this.selectedMeaning = null;
        this.showEditDialog = true;
    }

    editMeaning(meaning : Meaning) {
        this.selectedMeaning = meaning;
        this.showEditDialog = true;
    }

    deleteMeaning(meaning : Meaning) {
        this.dictionaryService.deleteMeaning(meaning.deleteLink)
        .subscribe(r => {
            this.getMeanings();
        }, this.handlerError);
    }

    onEditClosed(created : boolean){
        this.showEditDialog = false;
        if (created){
            this.getMeanings();
        }
    }

    handlerError(error : any) {
        this.errorMessage = <any>error;
        this.isLoading = false;
    }
}