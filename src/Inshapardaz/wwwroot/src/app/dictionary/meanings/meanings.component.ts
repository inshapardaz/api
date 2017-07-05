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

    editMeaning(meaning) {
        console.log("Edit meaning not implemented");
    }

    deleteMeaning(meaning) {
        if (meaning.deleteLink == null){
            return;
        }

        console.log("Delete meaning not implemented");
    }
}