import { Component, Input } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';

import { DictionaryService } from '../../services/dictionary.service';
import { Dictionary } from '../../models/dictionary'

@Component({
    selector: 'dictionaries',
    templateUrl: 'app/components/dictionary/dictionaries.html'
})

export class DictionariesComponent {
    isLoading: boolean = false;
    dictionariesLink : string;
    errorMessage: string;
    dictionaries: Dictionary[];
    createLink: string;

    public createForm = this.fb.group({
        name: ["", Validators.required],
        language: ["Urdu", Validators.required],
        isPublic: [true] 
    });

    constructor(private dictionaryService: DictionaryService, public fb: FormBuilder) {
    }

    ngOnInit() {
        this.getEntry()
    }

    getEntry() {
        this.isLoading = true;
        this.dictionaryService.getEntry()
            .subscribe(
            entry => {
                this.dictionariesLink = entry.dictionariesLink;
                this.isLoading = false;
                this.loadDictionaries();
            },
            this.handlerError);
    }

    public loadDictionaries(){
        console.log("loading dictionaries");
        this.isLoading = true;
        this.dictionaryService.getDictionaries(this.dictionariesLink)
            .subscribe(data => {
                this.dictionaries = data.dictionaries;
                this.createLink = data.createLink;
                this.isLoading = false;
            },
            this.handlerError);
    }

    doCreate(event) {
        console.log(event);
        console.log(this.createForm.value);
    }

    handlerError(error: any) {
        this.errorMessage = <any>error;
        this.isLoading = false;
    }
}