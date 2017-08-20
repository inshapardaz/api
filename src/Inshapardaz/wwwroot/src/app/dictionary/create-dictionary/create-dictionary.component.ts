import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { DictionaryService } from '../../../services/dictionary.service';
import { Dictionary } from '../../../models/Dictionary';
import { Languages } from '../../../models/language';

@Component({
    selector: 'create-dictionaries',
    templateUrl: './create-dictionary.component.html'
})
export class CreateDictionaryComponent {
    model = new Dictionary();
    languages : any[];
    languagesEnum = Languages;
    createLink : any;

    constructor(private dictionaryService: DictionaryService, 
                private router: Router) {
        this.languages = Object.keys(this.languagesEnum).filter(Number);
        this.model.language = Languages.Urdu;
    }  

    ngOnInit() {
        this.getEntry();
    }

    getEntry() {
        this.dictionaryService.getEntry()
            .subscribe(
            entry => { 
                this.dictionaryService.getDictionaries(entry.dictionariesLink)
                    .subscribe(data => {
                        if (!data.createLink){
                            this.router.navigate(['/error/unauthorised']);
                            return;
                        }
                        this.createLink = data.createLink;
                    },
                    this.handlerError);
            },
            this.handlerError);
    }

    onSubmit(){
        this.dictionaryService.createDictionary(this.createLink, this.model)
        .subscribe(m => {
            this.router.navigate(['/dictionaries']);
        },
        this.handlerCreationError);
    }

    handlerError(error : any) {
        
    }

    handlerCreationError(error : any) {
        
    }
}