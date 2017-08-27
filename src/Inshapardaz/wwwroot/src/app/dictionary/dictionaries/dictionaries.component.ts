import { Component } from '@angular/core';
import { Router } from '@angular/router';

import {TranslateService} from 'ng2-translate';
import {Languages} from '../../../models/language';
import { DictionaryService } from '../../../services/dictionary.service';
import { Dictionary } from '../../../models/dictionary';
import { AuthService } from '../../../services/auth.service';
import { AlertService } from '../../../services/alert.service';

 
@Component({
    selector: 'dictionaries',
    templateUrl: './dictionaries.component.html'
})

export class DictionariesComponent {
    isLoading : boolean = false;
    errorLoadingDictionaries : boolean = false;
    errorMessage: string;
    dictionaries : Dictionary[];
    createLink : string;
    dictionariesLink : string;
    showCreateDialog : boolean = false;
    selectedDictionary : Dictionary;
    Languages = Languages;

    constructor(private dictionaryService: DictionaryService, 
                private auth: AuthService,
                private alertService: AlertService,
                private router: Router,
                private translate: TranslateService){
    }

    ngOnInit() {
        this.getEntry();
    }

    deleteDictionary(dictionary : Dictionary) {
        this.dictionaryService.deleteDictionary(dictionary.deleteLink)
        .subscribe(r => {
            this.alertService.success(this.translate.instant('DICTIONARIES.MESSAGES.DELETION_SUCCESS'));
            this.getDictionaries();
        }, e => {
            this.handlerError();
            this.alertService.error(this.translate.instant('DICTIONARIES.MESSAGES.DELETION_FAILURE'));
        });
    }

    getEntry() {
        this.isLoading = true;
        this.dictionaryService.getEntry()
            .subscribe(
                entry => {
                    this.dictionariesLink = entry.dictionariesLink;
                    this.getDictionaries();
            }, e => {
                this.handlerError();
                this.router.navigate(['/error/servererror']);
            });
    }

    getDictionaries(){
        this.errorLoadingDictionaries = false;
        this.dictionaryService.getDictionaries(this.dictionariesLink)
        .subscribe(data => {
            this.dictionaries = data.dictionaries;
            this.createLink = data.createLink;
            this.isLoading = false;
        }, e => {            
            this.handlerError();
            this.errorLoadingDictionaries = true;
            this.alertService.error(this.translate.instant('DICTIONARIES.MESSAGES.LOADING_FAILURE'));
        });
    }

    createDictionary(){
        this.selectedDictionary = null;        
        this.showCreateDialog = true;
    }

    editDictionary(dictionary : Dictionary){
        this.selectedDictionary = dictionary;
        this.showCreateDialog = true;
    }

    createDictionaryDownload(dictionary : Dictionary){
        this.dictionaryService.createDictionaryDownload(dictionary.createDownloadLink)
        .subscribe(data => {
            this.alertService.success(this.translate.instant('DICTIONARIES.MESSAGES.CREATION_SUCCESS'));
        }, e => {
            this.handlerError(); 
            this.alertService.error(this.translate.instant('DICTIONARIES.MESSAGES.DOWNLOAD_REQUEST_FAILURE'));
        });
    }

    onCreateClosed(created : boolean){
        this.showCreateDialog = false;
        if (created){
            this.getDictionaries();
        }
    }

    handlerError() {
        this.isLoading = false;
    }
}
