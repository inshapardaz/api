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
    public createLink: string;

    public relationTypes : RelationTypes;
    public isLoading : boolean = false;
    public errorMessage: string;
    public relations : Array<Relation>;
    selectedRelation : Relation = null;
    showEditDialog : boolean = false;
    
    @Input()
    set relationsLink(relationsLink: string) {
        this._relationsLink = (relationsLink) || '';
        this.getRelations();
    }
    get relationsLink(): string { return this._relationsLink; }

    @Input()
    set createRelationLink(createLink: string) {
        this.createLink = (createLink) || '';
    }
    
    get createRelationLink(): string { return this.createLink; }

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

    addRelation(){
        this.selectedRelation = null;
        this.showEditDialog = true;
        console.log(this.showEditDialog);
        
    }

    editRelation(relation : Relation){
        this.selectedRelation = relation;
        this.showEditDialog = true;
        console.log(this.showEditDialog);
        
    }

    deleteRelation(relation : Relation){
        this.dictionaryService.deleteRelation(relation.deleteLink)
        .subscribe(r => {
            this.getRelations();
        }, this.handlerError);  
    }

    onEditClosed(created : boolean){
        this.showEditDialog = false;
        console.log(this.showEditDialog);
        
        if (created){
            this.getRelations();
        }
    }

    handlerError(error : any) {
        this.errorMessage = <any>error;
    }
}