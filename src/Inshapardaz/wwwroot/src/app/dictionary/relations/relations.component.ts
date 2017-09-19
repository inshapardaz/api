import { TranslateService } from '@ngx-translate/core';
import { AlertService } from '../../../services/alert.service';
import { Component, Input  } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { DictionaryService } from '../../../services/dictionary.service';
import { Relation } from '../../../models/relation';
import { Word } from '../../../models/Word';
import { RelationTypes } from '../../../models/relationTypes';

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
    
    @Input() createRelationLink : string;
    @Input() dictionaryLink : string;
    @Input() sourceWord : Word;
    @Input()
    set relationsLink(relationsLink: string) {
        this._relationsLink = (relationsLink) || '';
        this.getRelations();
    }
    get relationsLink(): string { return this._relationsLink; }
    
    constructor(private route: ActivatedRoute,
        private router: Router,
        private alertService: AlertService,
        private translate: TranslateService,
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
                this.alertService.error(this.translate.instant('RELATION.MESSAGES.LOAD_FAILURE'));                
                this.errorMessage = <any>error;
            });
    }

    addRelation(){
        this.selectedRelation = null;
        this.showEditDialog = true;
    }

    editRelation(relation : Relation){
        this.selectedRelation = relation;
        this.showEditDialog = true;
    }

    deleteRelation(relation : Relation){
        this.dictionaryService.deleteRelation(relation.deleteLink)
        .subscribe(r => {
            this.alertService.success(this.translate.instant('RELATION.MESSAGES.DELETE_SUCCESS'));            
            this.getRelations();
        }, error => {
            this.errorMessage = <any>error;   
            this.alertService.error(this.translate.instant('RELATION.MESSAGES.DELETE_FAILURE'));
        });  
    }

    onEditClosed(created : boolean){
        this.showEditDialog = false;
        
        if (created){
            this.getRelations();
        }
    }
}