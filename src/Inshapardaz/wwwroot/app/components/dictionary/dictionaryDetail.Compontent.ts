import { Component, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';

import { DictionaryService } from '../../services/dictionary.service';
import { Dictionary } from '../../models/Dictionary';

@Component({
    selector : 'dictionary-detail',
    templateUrl : 'app/components/dictionary/dictionaryDetail.html'
})

export class DictionaryDetailComponent{
        private sub: Subscription;
    id : number;
    isWord : boolean;
    errorMessage : string;
    dictionary : Dictionary;

    constructor(private route: ActivatedRoute,
        private router: Router,
        private dictionaryService: DictionaryService){
    }

    ngOnInit() {
        this.sub = this.route.params.subscribe(params => {
            this.id = params['id'];
            this.getDictionary();
        });
    }

    getDictionary(){
        this.isWord = true;
        this.dictionaryService.getDictionary(this.id)
            .subscribe(
            dict => { 
                this.dictionary = dict;
            },
            error => {
                this.errorMessage = <any>error;
                this.isWord = false;
            });
    }
}