import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

import * as _ from 'lodash';

import { Mapper } from '../mapper';
import { Dictionaries } from '../models/dictionaries';
import { Dictionary } from '../models/dictionary';
import { Link } from '../models/link';
import { Word } from '../models/word';
import { WordDetail } from '../models/worddetail';
import { WordPage } from '../models/wordpage';
import { Meaning } from '../models/meaning';
import { Relation } from '../models/relation';
import { Translation } from '../models/translation';
import { Entry } from '../models/entry';

@Injectable()
export class DictionaryService {
    private serverAddress = '';
    private entryUrl = this.serverAddress + '/api';
    private indexUrl = this.serverAddress + '/api/dictionary/index';
    private dictionaryUrl = this.serverAddress + '/api/dictionaries/';
    private wordUrl = this.serverAddress + '/api/words/';
    private searchUrl = '/api/words/search/';
    private staringWithUrl = '/api/words/StartWith/';
    private static _entry : Entry;

    constructor(private auth: AuthService, 
        private http: Http) {
        let sessionOverride = sessionStorage.getItem('server-address');
        if (sessionOverride !== null){
            this.serverAddress = sessionOverride;
            console.log('using local override : '+ this.serverAddress);            
            this.entryUrl = this.serverAddress + '/api';
            this.indexUrl = this.serverAddress + '/api/dictionary/index';
            this.dictionaryUrl = this.serverAddress + '/api/dictionaries/';
            this.wordUrl = this.serverAddress + '/api/words/';
            this.searchUrl = '/api/words/search/';
            this.staringWithUrl = '/api/words/StartWith/';
        }
    }

    getEntry(): Observable<Entry> {
        return this.auth.AuthGet(this.entryUrl, this.createOptions())
            .map(r => {
                var e = this.extractData(r, Mapper.MapEntry);
                DictionaryService._entry = e;
                return e;
            })
            .catch(this.handleError);
    }

    getDictionaries(link: string): Observable<Dictionaries> {
        return this.auth.AuthGet(link, this.createOptions())
            .map(r => this.extractData(r, Mapper.MapDictionaries))
            .catch(this.handleError);
    }

    createDictionary(createLink : string, dictionary : Dictionary) : Observable<Dictionary>{
        return this.auth.AuthPost(createLink, JSON.stringify(dictionary), this.createOptions())
            .map(r => this.extractData(r, Mapper.MapDictionaries))
            .catch(this.handleError);
    }
    
    updateDictionary(updateLink : string, dictionary : Dictionary) : Observable<void>{
        return this.auth.AuthPut(updateLink, JSON.stringify(dictionary), this.createOptions())
            .catch(this.handleError);
    }

    deleteDictionary(deleteLink : string) : Observable<void>{
        return this.auth.AuthDelete(deleteLink, this.createOptions())
            .catch(this.handleError);
    }

    createDictionaryDownload(createDownloadLink : string) : Observable<void>{
        return this.auth.AuthPost(createDownloadLink, this.createOptions())
            .catch(this.handleError);
    }
    getDictionary(id: number): Observable<Dictionary> {
        return this.auth.AuthGet(this.dictionaryUrl + id, this.createOptions())
            .map(r => this.extractData(r, Mapper.MapDictionary))
            .catch(this.handleError);
    }

    searchWords(url: string, query: string, pageNumber: number = 1, pageSize: number = 10): Observable<WordPage> {
        return this.auth.AuthGet(url + "?query=" + query + "&pageNumber=" + pageNumber + "&pageSize=" + pageSize, this.createOptions())
            .map(r => this.extractData(r, Mapper.MapWordPage))
            .catch(this.handleError);
    }

    getWords(url: string, pageNumber: number = 1, pageSize: number = 10): Observable<WordPage> {
        return this.auth.AuthGet(url + "?pageNumber=" + pageNumber + "&pageSize=" + pageSize, this.createOptions())
            .map(r => this.extractData(r, Mapper.MapWordPage))
            .catch(this.handleError);
    }

    getWordById(wordId): Observable<Word> {
        return this.auth.AuthGet(this.wordUrl + wordId, this.createOptions())
            .map(r => this.extractData(r, Mapper.MapWord))
            .catch(this.handleError);
    }

    getWord(url: string): Observable<Word> {
        return this.auth.AuthGet(url)
            .map(r => this.extractData(r, Mapper.MapWord))
            .catch(this.handleError);
    }

    createWord(createWordLink : string, word : Word) : Observable<void>{
        return this.auth.AuthPost(createWordLink, JSON.stringify(word), this.createOptions())
            .catch(this.handleError);
    }

    updateWord(updateLink : string, word : Word) : Observable<void>{
        return this.auth.AuthPut(updateLink, JSON.stringify(word), this.createOptions())
            .catch(this.handleError);
    }

    deleteWord(deleteLink : string) : Observable<void>{
        return this.auth.AuthDelete(deleteLink)
            .catch(this.handleError);
    }

    searchWord(searchText: string, pageNumber: number = 1, pageSize: number = 10): Observable<WordPage> {
        return this.auth.AuthGet(this.searchUrl + searchText + "?pageNumber=" + pageNumber + "&pageSize=" + pageSize, this.createOptions())
            .map(r => this.extractData(r, Mapper.MapWordPage))
            .catch(this.handleError);
    }

    getSearchResults(url: string): Observable<WordPage> {
        return this.auth.AuthGet(url, this.createOptions())
            .map(r => this.extractData(r, Mapper.MapWordPage))
            .catch(this.handleError);
    }

    wordStartingWith(startingWith: string, pageNumber: number = 1, pageSize: number = 10): Observable<WordPage> {
        return this.auth.AuthGet(this.staringWithUrl + startingWith + "?pageNumber=" + pageNumber + "&pageSize=" + pageSize, this.createOptions())
            .map(r => this.extractData(r, Mapper.MapWordPage))
            .catch(this.handleError);
    }

    getWordStartingWith(url: string, pageNumber: number = 1, pageSize: number = 10): Observable<WordPage> {
        return this.auth.AuthGet(url+ "?pageNumber=" + pageNumber + "&pageSize=" + pageSize, this.createOptions())
            .map(r => this.extractData(r, Mapper.MapWordPage))
            .catch(this.handleError);
    }

    getWordRelations(url: string): Observable<Array<Relation>> {
        return this.auth.AuthGet(url, this.createOptions())
            .map(r => this.extractData(r, Mapper.MapRelations))
            .catch(this.handleError);
    }

    getWordTranslations(url: string): Observable<Array<Translation>> {
        return this.auth.AuthGet(url, this.createOptions())
            .map(r => this.extractData(r, Mapper.MapTranslations))
            .catch(this.handleError);
    }

    getMeanings(url: string): Observable<Array<Meaning>> {
        return this.auth.AuthGet(url, this.createOptions())
            .map(r => this.extractData(r, Mapper.MapMeanings))
            .catch(this.handleError);
    }

    getWordDetails(url: string): Observable<Array<WordDetail>> {
        return this.auth.AuthGet(url, this.createOptions())
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

    private createOptions(): RequestOptions{
        return null;
        // let headers = new Headers();
        // headers.append('Accept-Type', 'application/json');
        // headers.append('Content-Type', 'application/json');
    
        // return new RequestOptions({
        //     headers: headers
        // }); 
    }
}
