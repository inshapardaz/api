import {Component, Input} from '@angular/core'
import { DictionaryService } from '../../services/dictionary.service';
import { Dictionary } from '../../models/dictionary'

@Component({
    selector : 'dictionary-overview',
    templateUrl : 'app/components/dictionary/overview.html'
})

export class DictionaryOverviewComponent{
    isLoading : boolean = false;
    errorMessage: string;
    dictionaries : Dictionary[];
    createLink : string;

    constructor(private dictionaryService: DictionaryService){
    }

    ngOnInit() {
        //this.getEntry()
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
                    },
                    this.handlerError);
                this.isLoading = false;
            },
            this.handlerError);
    }

    handlerError(error : any) {
        this.errorMessage = <any>error;
        this.isLoading = false;
    }
}