import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { AuthService } from './auth.service';
import { AuthHttp } from 'angular2-jwt';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

import * as _ from 'lodash';

import { Mapper } from '../mapper';
//import { DictionaryIndex } from '../models/dictionaryIndex';
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

    constructor(private auth: AuthService, private authHttp: AuthHttp, private http: Http) {
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
        console.debug('Calling ' + this.entryUrl);
        return this.getHttp().get(this.entryUrl)
            .map(r => {
                var e = this.extractData(r, Mapper.MapEntry);
                DictionaryService._entry = e;
                return e;
            })
            .catch(this.handleError);
    }

    getDictionaries(link: string): Observable<Dictionaries> {
        return this.getHttp().get(link)
            .map(r => this.extractData(r, Mapper.MapDictionaries))
            .catch(this.handleError);
    }

    createDictionary(createLink : string, dictionary : Dictionary) : Observable<Dictionary>{
        let headers = new Headers();
        headers.append('Accept-Type', 'application/json');
        headers.append('Content-Type', 'application/json');
    
        let options = new RequestOptions({
            headers: headers
        });
        
        return this.getHttp().post(createLink, JSON.stringify(dictionary), options)
            .map(r => this.extractData(r, Mapper.MapDictionaries))
            .catch(this.handleError);
    }
    
    updateDictionary(updateLink : string, dictionary : Dictionary) : Observable<void>{
        let headers = new Headers();
        headers.append('Accept-Type', 'application/json');
        headers.append('Content-Type', 'application/json');
    
        let options = new RequestOptions({
            headers: headers
        });
        
        return this.getHttp().put(updateLink, JSON.stringify(dictionary), options)
            .catch(this.handleError);
    }

    deleteDictionary(deleteLink : string) : Observable<void>{
        return this.getHttp().delete(deleteLink)
            .catch(this.handleError);
    }

    createDictionaryDownload(createDownloadLink : string) : Observable<void>{
        return this.getHttp().post(createDownloadLink, {})
            .catch(this.handleError);
    }
    getDictionary(id: number): Observable<Dictionary> {
        return this.getHttp().get(this.dictionaryUrl + id)
            .map(r => this.extractData(r, Mapper.MapDictionary))
            .catch(this.handleError);
    }

    searchWords(url: string, query: string, pageNumber: number = 1, pageSize: number = 10): Observable<WordPage> {
        return this.getHttp().get(url + "?query=" + query + "&pageNumber=" + pageNumber + "&pageSize=" + pageSize)
            .map(r => this.extractData(r, Mapper.MapWordPage))
            .catch(this.handleError);
    }

    getWords(url: string, pageNumber: number = 1, pageSize: number = 10): Observable<WordPage> {
        return this.getHttp().get(url + "?pageNumber=" + pageNumber + "&pageSize=" + pageSize)
            .map(r => this.extractData(r, Mapper.MapWordPage))
            .catch(this.handleError);
    }

    // getIndex(): Observable<DictionaryIndex> {
    //     return this.getHttp().get(this.indexUrl)
    //         .map(r => this.extractData(r, Mapper.MapDictionaryIndex))
    //         .catch(this.handleError);
    // }

    getWordById(wordId): Observable<Word> {
        return this.getHttp().get(this.wordUrl + wordId)
            .map(r => this.extractData(r, Mapper.MapWord))
            .catch(this.handleError);
    }

    getWord(url: string): Observable<Word> {
        return this.getHttp().get(url)
            .map(r => this.extractData(r, Mapper.MapWord))
            .catch(this.handleError);
    }

    searchWord(searchText: string, pageNumber: number = 1, pageSize: number = 10): Observable<WordPage> {
        return this.getHttp().get(this.searchUrl + searchText + "?pageNumber=" + pageNumber + "&pageSize=" + pageSize)
            .map(r => this.extractData(r, Mapper.MapWordPage))
            .catch(this.handleError);
    }

    getSearchResults(url: string): Observable<WordPage> {
        return this.getHttp().get(url)
            .map(r => this.extractData(r, Mapper.MapWordPage))
            .catch(this.handleError);
    }

    wordStartingWith(startingWith: string, pageNumber: number = 1, pageSize: number = 10): Observable<WordPage> {
        return this.getHttp().get(this.staringWithUrl + startingWith + "?pageNumber=" + pageNumber + "&pageSize=" + pageSize)
            .map(r => this.extractData(r, Mapper.MapWordPage))
            .catch(this.handleError);
    }

    getWordStartingWith(url: string): Observable<WordPage> {
        return this.getHttp().get(url)
            .map(r => this.extractData(r, Mapper.MapWordPage))
            .catch(this.handleError);
    }

    getWordRelations(url: string): Observable<Array<Relation>> {
        return this.getHttp().get(url)
            .map(r => this.extractData(r, Mapper.MapRelations))
            .catch(this.handleError);
    }

    getWordTranslations(url: string): Observable<Array<Translation>> {
        return this.getHttp().get(url)
            .map(r => this.extractData(r, Mapper.MapTranslations))
            .catch(this.handleError);
    }

    getMeanings(url: string): Observable<Array<Meaning>> {
        return this.getHttp().get(url)
            .map(r => this.extractData(r, Mapper.MapMeanings))
            .catch(this.handleError);
    }

    getWordDetails(url: string): Observable<Array<WordDetail>> {
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

    private getHttp() {
        return this.auth.isAuthenticated() ? this.authHttp : this.http;
    }
}
