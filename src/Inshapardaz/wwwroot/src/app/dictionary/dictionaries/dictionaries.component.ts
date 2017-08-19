import { Component } from '@angular/core';
import { DictionaryService } from '../../../services/dictionary.service';
import { Dictionary } from '../../../models/dictionary';
import { AuthService } from '../../../services/auth.service';

import { CreateDictionariesComponent } from '../create-dictionary/create-dictionary.component';

import { DialogService } from "ng2-bootstrap-modal";


@Component({
    selector: 'dictionaries',
    templateUrl: './dictionaries.component.html'
})

export class DictionariesComponent {
    isLoading : boolean = false;
    errorMessage: string;
    dictionaries : Dictionary[];
    createLink : string;

    constructor(private dictionaryService: DictionaryService, 
                private dialogService:DialogService,
                private auth: AuthService){
    }

    ngOnInit() {
        this.getEntry();
    }

    createDictionary() {
        let disposable = this.dialogService.addDialog(CreateDictionariesComponent, {
            title:'Confirm title', 
            message:'Confirm message'})
            .subscribe((isConfirmed)=>{
                //We get dialog result
                if(isConfirmed) {
                    alert('accepted');
                }
                else {
                    alert('declined');
                }
            });
        //We can close dialog calling disposable.unsubscribe();
        //If dialog was not closed manually close it by timeout
        setTimeout(()=>{
            disposable.unsubscribe();
        },10000);
    }


    getEntry() {
        this.isLoading = true;
        this.dictionaryService.getEntry()
            .subscribe(
            entry => { 
                this.dictionaryService.getDictionaries(entry.dictionariesLink)
                    .subscribe(data => {
                        this.dictionaries = data.dictionaries;
                        this.createLink = data.createLink;
                        this.isLoading = false;
                    },
                    this.handlerError);
            },
            this.handlerError);
    }

    handlerError(error : any) {
        this.errorMessage = <any>error;
        this.isLoading = false;
    }
}
