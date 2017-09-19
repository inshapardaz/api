import { AlertService } from '../../../services/alert.service';
import { TranslateService } from '@ngx-translate/core';
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

    isBusy : boolean = false;
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
        private alertService: AlertService,
        private translate: TranslateService,
        private dictionaryService: DictionaryService){
    }

    getWordDetails() {
        this.isBusy = true;
        this.dictionaryService.getWordDetails(this._wordDetailLink)
            .subscribe(
            details => { 
                this.wordDetails = details;
                this.isBusy = false;
            },
            error => {
                this.isBusy = false;
                this.alertService.error(this.translate.instant('WORDDETAIL.MESSAGES.LOAD_FAILURE'));                
                this.errorMessage = <any>error;
            });
    }
    
    editDetail(detail : WordDetail){
        this.selectedDetail = detail;
        this.showEditDialog = true;
    }

    deleteDetail(detail : WordDetail){
        this.isBusy = true;
        this.dictionaryService.deleteWordDetail(detail.deleteLink)
        .subscribe(r => {
            this.isBusy = false;
            this.alertService.success(this.translate.instant('WORDDETAIL.MESSAGES.DELETE_SUCCESS'));
            this.getWordDetails();
        }, error => {
            this.isBusy = false;
            this.alertService.error(this.translate.instant('WORDDETAIL.MESSAGES.DELETE_FAILURE'));
            this.errorMessage = <any>error;
        });  
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
}