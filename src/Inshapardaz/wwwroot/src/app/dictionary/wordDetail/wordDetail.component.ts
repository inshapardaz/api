import { Component, Input  } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { DictionaryService } from '../../../services/dictionary.service';
import { WordDetail } from '../../../models/worddetail';

@Component({
    selector: 'word-detail',
    templateUrl: './wordDetail.html'
})

export class WordDetailsComponent {
    _wordDetailLink: string;
    selectedDetail : WordDetail;
    showEditDialog : boolean = false;
    @Input() createLink: string = '';

    isLoadingDetails : boolean = false;
    errorMessage: string;
    wordDetails : Array<WordDetail>;

    @Input()
    set wordDetailLink(wordDetailLink: string) {
        this._wordDetailLink = (wordDetailLink) || '';
        this.getWordDetails();
    }
    
    get wordDetailLink(): string { return this._wordDetailLink; }

    constructor(private route: ActivatedRoute,
        private router: Router,
        private dictionaryService: DictionaryService){
    }

    editDetail(detail : WordDetail){
        this.selectedDetail = detail;
        this.showEditDialog = true;
    }

    deleteDetail(detail : WordDetail){
        this.dictionaryService.deleteWordDetail(detail.deleteLink)
        .subscribe(r => {
            this.getWordDetails();
        }, this.handlerError);  
    }

    addDetail(){
        this.selectedDetail = null;
        this.showEditDialog = true;        
    }

    onEditClosed(created : boolean){
        this.showEditDialog = false;
        if (created){
            this.getWordDetails();
        }
    }

    getWordDetails() {
        this.isLoadingDetails = true;
        this.dictionaryService.getWordDetails(this._wordDetailLink)
            .subscribe(
            details => { 
                this.wordDetails = details;
                this.isLoadingDetails = false;
            },
            error => {
                this.errorMessage = <any>error;
            });
    }

    handlerError(error : any) {
        this.errorMessage = <any>error;
    }
}