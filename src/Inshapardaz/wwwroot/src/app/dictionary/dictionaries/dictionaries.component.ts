import { Component } from '@angular/core';
import { DictionaryService } from '../../../services/dictionary.service';
import { Dictionary } from '../../../models/dictionary';
import { AuthService } from '../../../services/auth.service';

@Component({
    selector: 'dictionaries',
    templateUrl: './dictionaries.component.html'
})

export class DictionariesComponent {
    isLoading : boolean = false;
    errorMessage: string;
    dictionaries : Dictionary[];
    createLink : string;
    dictionariesLink : string;
    showCreateDialog : boolean = false;
    selectedDictionary : Dictionary;

    constructor(private dictionaryService: DictionaryService, 
                private auth: AuthService){
    }

    ngOnInit() {
        this.getEntry();
    }

    deleteDictionary(dictionary : Dictionary) {
        this.dictionaryService.deleteDictionary(dictionary.deleteLink)
        .subscribe(r => {
            this.getDictionaries();
        });
    }

    getEntry() {
        this.isLoading = true;
        this.dictionaryService.getEntry()
            .subscribe(
                entry => {
                    this.dictionariesLink = entry.dictionariesLink;
                    this.getDictionaries();
            },
            this.handlerError);
    }

    getDictionaries(){
        this.dictionaryService.getDictionaries(this.dictionariesLink)
        .subscribe(data => {
            this.dictionaries = data.dictionaries;
            this.createLink = data.createLink;
            this.isLoading = false;
        },
        this.handlerError);
    }

    createDictionary(){
        this.selectedDictionary = null;        
        this.showCreateDialog = true;
    }

    editDictionary(dictionary : Dictionary){
        console.log("editing dictionary :" + dictionary.id);
        this.selectedDictionary = dictionary;
        this.showCreateDialog = true;
    }

    createDictionaryDownload(dictionary : Dictionary){
        this.dictionaryService.createDictionaryDownload(dictionary.createDownloadLink)
        .subscribe(data => {
            // TODO : Raise a success message
        },
        this.handlerError);
    }

    onCreateClosed(created : boolean){
        console.debug('create dialog closed');        
        this.showCreateDialog = false;
        if (created){
            this.getDictionaries();
        }
    }

    handlerError(error : any) {
        this.errorMessage = <any>error;
        this.isLoading = false;
    }
}
