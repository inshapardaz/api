import { Injectable } from '@angular/core';
import { Http, Response  } from '@angular/http';
import { Auth } from './auth.service';
import { AuthHttp } from 'angular2-jwt';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

import * as _ from 'lodash';

import { Mapper } from '../mapper';
import { DictionaryIndex } from '../models/dictionaryIndex';
import { Dictionaries } from '../models/dictionaries';
import { Dictionary } from '../models/dictionary';
import { Link } from '../models/link';
import { Word } from '../models/word';
import { WordDetail } from '../models/worddetail';
import { WordPage } from '../models/wordpage';
import { Relation } from '../models/relation';
import { Entry } from '../models/entry';

@Injectable()
export class DictionaryService {
    private entryUrl = '/api/entry';
    private indexUrl = '/api/dictionary/index';
    private dictionaryUrl = '/api/dictionary/';
    private wordUrl = '/api/word/';
    private searchUrl = '/api/words/search/';
    private staringWithUrl = '/api/words/StartWith/';

    constructor(private auth: Auth, private authHttp: AuthHttp, private http : Http) {
    }

    getEntry() : Observable<Entry>{
        return this.getHttp().get(this.entryUrl)
            .map(r => this.extractData(r, Mapper.MapEntry))
            .catch(this.handleError);
    }
    
    getDictionaries(link : string) : Observable<Dictionaries>{
        return this.getHttp().get(link)
            .map(r => this.extractData(r, Mapper.MapDictionaries))
            .catch(this.handleError);
    }

    getDictionary(id : number) : Observable<Dictionary> {
        return this.getHttp().get(this.dictionaryUrl + id)
            .map(r => this.extractData(r, Mapper.MapDictionary))
            .catch(this.handleError);
    }

    getIndex(): Observable<DictionaryIndex> {
        return this.getHttp().get(this.indexUrl)
            .map(r => this.extractData(r, Mapper.MapDictionaryIndex))
            .catch(this.handleError);
    }

    getWordByid(wordId): Observable<Word> {
        return this.getHttp().get(this.wordUrl + wordId)
            .map(r => this.extractData(r, Mapper.MapWord))
            .catch(this.handleError);
    }

    getWord(url : string): Observable<Word> {
        return this.getHttp().get(url)
            .map(r => this.extractData(r, Mapper.MapWord))
            .catch(this.handleError);
    }

    searchWord(searchText: string, pageNumber : number = 1, pageSize : number = 10): Observable<WordPage> {
        return this.getHttp().get(this.searchUrl + searchText + "?pageNumber=" + pageNumber + "&pageSize=" + pageSize )
            .map(r => this.extractData(r, Mapper.MapWordPage))
            .catch(this.handleError);
    }

    getSearchResults(url : string): Observable<WordPage> {
        return this.getHttp().get(url)
            .map(r => this.extractData(r, Mapper.MapWordPage))
            .catch(this.handleError);
    }

    wordStartingWith(startingWith: string, pageNumber : number = 1, pageSize : number = 10): Observable<WordPage> {
        return this.getHttp().get(this.staringWithUrl + startingWith + "?pageNumber=" + pageNumber + "&pageSize=" + pageSize )
            .map(r => this.extractData(r, Mapper.MapWordPage))
            .catch(this.handleError);
    }

    getWordStartingWith(url: string): Observable<WordPage> {
        return this.getHttp().get(url)
            .map(r => this.extractData(r, Mapper.MapWordPage))
            .catch(this.handleError);
    }

    getWordRelations(url : string) : Observable<Array<Relation>>{
        return this.getHttp().get(url)
            .map(r => this.extractData(r, Mapper.MapRelations))
            .catch(this.handleError);
    }

    getWordDetails(url : string) : Observable<Array<WordDetail>>{
        return this.getHttp().get(url)
            .map(r => this.extractData(r, Mapper.MapWordDetails))
            .catch(this.handleError);
    }

    private extractData(res: Response, converter: Function) {
        let body = res.json();
        return converter(body);
    }

    private handleError(error: any) {
        let errMsg = (error.message) ? error.message :
            error.status ? `${error.status} - ${error.statusText}` : 'Server error';
        console.error(errMsg); // log to console instead
        if (error.stack)
            console.error(error.stack);
        return Observable.throw(errMsg);
    }

    private getHttp(){
        return this.auth.authenticated() ? this.authHttp : this.http; 
    }
}