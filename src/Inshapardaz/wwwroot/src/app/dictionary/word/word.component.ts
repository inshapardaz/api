import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { FormsModule , FormBuilder } from '@angular/forms';

import { DictionaryService } from '../../../services/dictionary.service';
import { Word } from '../../../models/Word';

@Component({
    selector: 'word',
    templateUrl: './word.component.html'
})

export class WordComponent {
    private sub: Subscription;
    isLoading : boolean = false;
    errorMessage: string;
    id : number;
    word : Word;
    
    constructor(private route: ActivatedRoute,
        private router: Router,
        private dictionaryService: DictionaryService){
    }
    ngOnInit() {
        this.sub = this.route.params.subscribe(params => {
            this.id = params['id'];
            this.getWord();
        });
    }

    getWord() {
        this.isLoading = true;
        this.dictionaryService.getWordById(this.id)
            .subscribe(
            word => { 
                this.word = word;
                this.isLoading = false;
            },
            error => {
                this.errorMessage = <any>error;
            });
    }

    handlerError(error : any) {
        this.errorMessage = <any>error;
        this.isLoading = false;
    }
}
