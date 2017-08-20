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

    constructor(private dictionaryService: DictionaryService, 
                private auth: AuthService){
    }

    ngOnInit() {
        this.getEntry();
    }

    createDictionary() {
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
