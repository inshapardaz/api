import { Component, Input  } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { DictionaryService } from '../../../services/dictionary.service';
import { Relation } from '../../../models/relation';
import {RelationTypes} from '../../../models/relationTypes';

@Component({
    selector: 'word-relations',
    templateUrl: './Relations.html'
})

export class RelationsComponent {
    public _relationsLink: string;
    public _createLink: string;

    public relationTypes : RelationTypes;
    public isLoading : boolean = false;
    public errorMessage: string;
    public relations : Array<Relation>;

    @Input()
    set relationsLink(relationsLink: string) {
        this._relationsLink = (relationsLink) || '';
        this.getRelations();
    }
    get relationsLink(): string { return this._relationsLink; }

    @Input()
    set createRelationLink(createLink: string) {
        this._createLink = (createLink) || '';
    }
    
    get createRelationLink(): string { return this._createLink; }

    constructor(private route: ActivatedRoute,
        private router: Router,
        private dictionaryService: DictionaryService){
    }

    getRelations() {
        this.isLoading = true;
        this.dictionaryService.getWordRelations(this._relationsLink)
            .subscribe(
            relations => { 
                this.relations = relations;
                this.isLoading = false;
            },
            error => {
                this.errorMessage = <any>error;
            });
    }
}