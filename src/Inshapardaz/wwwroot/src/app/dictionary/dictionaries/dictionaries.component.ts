import { Component } from '@angular/core';
import { Router } from '@angular/router';

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

    constructor(private dictionaryService: DictionaryService, 
                private auth: AuthService,
                private alertService: AlertService,
                private router: Router,){
    }

    ngOnInit() {
        this.getEntry();
    }

    deleteDictionary(dictionary : Dictionary) {
        this.dictionaryService.deleteDictionary(dictionary.deleteLink)
        .subscribe(r => {
            this.alertService.success('Dictionary ' + dictionary.name + ' deleted successfully.' );
            this.getDictionaries();
        }, e => {
            this.handlerError();
            this.alertService.error('Unable to delete dictionary ' + dictionary.name + '.' );            
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
            this.alertService.error('Unable to load dictionaries. Please try again');
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
            this.alertService.success('Dictionary download request sent.');
        }, e => {
            this.handlerError(); 
            this.alertService.error('Dictionary download request failed. Please try again.');            
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
