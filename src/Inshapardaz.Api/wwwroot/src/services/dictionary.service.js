var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { Mapper } from '../mapper';
var DictionaryService = DictionaryService_1 = (function () {
    function DictionaryService(auth, http) {
        this.auth = auth;
        this.http = http;
        this.serverAddress = '';
        this.entryUrl = this.serverAddress + '/api';
        this.indexUrl = this.serverAddress + '/api/dictionary/index';
        this.dictionaryUrl = this.serverAddress + '/api/dictionaries/';
        this.wordUrl = this.serverAddress + '/api/words/';
        this.searchUrl = '/api/words/search/';
        this.staringWithUrl = '/api/words/StartWith/';
        var sessionOverride = sessionStorage.getItem('server-address');
        if (sessionOverride !== null) {
            this.serverAddress = sessionOverride;
            console.debug('using local override : ' + this.serverAddress);
            this.entryUrl = this.serverAddress + '/api';
            this.indexUrl = this.serverAddress + '/api/dictionary/index';
            this.dictionaryUrl = this.serverAddress + '/api/dictionaries/';
            this.wordUrl = this.serverAddress + '/api/words/';
            this.searchUrl = '/api/words/search/';
            this.staringWithUrl = '/api/words/StartWith/';
        }
    }
    DictionaryService.prototype.getEntry = function () {
        var _this = this;
        return this.auth.AuthGet(this.entryUrl)
            .map(function (r) {
            var e = _this.extractData(r, Mapper.MapEntry);
            DictionaryService_1._entry = e;
            return e;
        })
            .catch(this.handleError);
    };
    DictionaryService.prototype.getDictionaries = function (link) {
        var _this = this;
        return this.auth.AuthGet(link)
            .map(function (r) { return _this.extractData(r, Mapper.MapDictionaries); })
            .catch(this.handleError);
    };
    DictionaryService.prototype.createDictionary = function (createLink, dictionary) {
        var _this = this;
        return this.auth.AuthPost(createLink, dictionary)
            .map(function (r) { return _this.extractData(r, Mapper.MapDictionaries); })
            .catch(this.handleError);
    };
    DictionaryService.prototype.updateDictionary = function (updateLink, dictionary) {
        return this.auth.AuthPut(updateLink, dictionary)
            .catch(this.handleError);
    };
    DictionaryService.prototype.deleteDictionary = function (deleteLink) {
        return this.auth.AuthDelete(deleteLink)
            .catch(this.handleError);
    };
    DictionaryService.prototype.createDictionaryDownload = function (createDownloadLink) {
        return this.auth.AuthPost(createDownloadLink, {})
            .catch(this.handleError);
    };
    DictionaryService.prototype.getDictionary = function (id) {
        var _this = this;
        return this.auth.AuthGet(this.dictionaryUrl + id)
            .map(function (r) { return _this.extractData(r, Mapper.MapDictionary); })
            .catch(this.handleError);
    };
    DictionaryService.prototype.searchWords = function (url, query, pageNumber, pageSize) {
        var _this = this;
        if (pageNumber === void 0) { pageNumber = 1; }
        if (pageSize === void 0) { pageSize = 10; }
        return this.auth.AuthGet(url + "?query=" + query + "&pageNumber=" + pageNumber + "&pageSize=" + pageSize)
            .map(function (r) { return _this.extractData(r, Mapper.MapWordPage); })
            .catch(this.handleError);
    };
    DictionaryService.prototype.getWords = function (url, pageNumber, pageSize) {
        var _this = this;
        if (pageNumber === void 0) { pageNumber = 1; }
        if (pageSize === void 0) { pageSize = 10; }
        return this.auth.AuthGet(url + "?pageNumber=" + pageNumber + "&pageSize=" + pageSize)
            .map(function (r) { return _this.extractData(r, Mapper.MapWordPage); })
            .catch(this.handleError);
    };
    DictionaryService.prototype.getWordById = function (wordId) {
        var _this = this;
        return this.auth.AuthGet(this.wordUrl + wordId)
            .map(function (r) { return _this.extractData(r, Mapper.MapWord); })
            .catch(this.handleError);
    };
    DictionaryService.prototype.getWord = function (url) {
        var _this = this;
        return this.auth.AuthGet(url)
            .map(function (r) { return _this.extractData(r, Mapper.MapWord); })
            .catch(this.handleError);
    };
    DictionaryService.prototype.createWord = function (createWordLink, word) {
        return this.auth.AuthPost(createWordLink, word)
            .catch(this.handleError);
    };
    DictionaryService.prototype.updateWord = function (updateLink, word) {
        return this.auth.AuthPut(updateLink, word)
            .catch(this.handleError);
    };
    DictionaryService.prototype.deleteWord = function (deleteLink) {
        return this.auth.AuthDelete(deleteLink)
            .catch(this.handleError);
    };
    DictionaryService.prototype.searchWord = function (searchText, pageNumber, pageSize) {
        var _this = this;
        if (pageNumber === void 0) { pageNumber = 1; }
        if (pageSize === void 0) { pageSize = 10; }
        return this.auth.AuthGet(this.searchUrl + searchText + "?pageNumber=" + pageNumber + "&pageSize=" + pageSize)
            .map(function (r) { return _this.extractData(r, Mapper.MapWordPage); })
            .catch(this.handleError);
    };
    DictionaryService.prototype.getSearchResults = function (url) {
        var _this = this;
        return this.auth.AuthGet(url)
            .map(function (r) { return _this.extractData(r, Mapper.MapWordPage); })
            .catch(this.handleError);
    };
    DictionaryService.prototype.wordStartingWith = function (startingWith, pageNumber, pageSize) {
        var _this = this;
        if (pageNumber === void 0) { pageNumber = 1; }
        if (pageSize === void 0) { pageSize = 10; }
        return this.auth.AuthGet(this.staringWithUrl + startingWith + "?pageNumber=" + pageNumber + "&pageSize=" + pageSize)
            .map(function (r) { return _this.extractData(r, Mapper.MapWordPage); })
            .catch(this.handleError);
    };
    DictionaryService.prototype.getWordStartingWith = function (url, pageNumber, pageSize) {
        var _this = this;
        if (pageNumber === void 0) { pageNumber = 1; }
        if (pageSize === void 0) { pageSize = 10; }
        return this.auth.AuthGet(url + "?pageNumber=" + pageNumber + "&pageSize=" + pageSize)
            .map(function (r) { return _this.extractData(r, Mapper.MapWordPage); })
            .catch(this.handleError);
    };
    DictionaryService.prototype.getWordsStartingWith = function (url, startWith, pageNumber, pageSize) {
        var _this = this;
        if (pageNumber === void 0) { pageNumber = 1; }
        if (pageSize === void 0) { pageSize = 10; }
        return this.auth.AuthGet(url + "/words/startWith/" + startWith + "?pageNumber=" + pageNumber + "&pageSize=" + pageSize)
            .map(function (r) { return _this.extractData(r, Mapper.MapWordPage).words; })
            .catch(this.handleError);
    };
    DictionaryService.prototype.getWordRelations = function (url) {
        var _this = this;
        return this.auth.AuthGet(url)
            .map(function (r) { return _this.extractData(r, Mapper.MapRelations); })
            .catch(this.handleError);
    };
    DictionaryService.prototype.getWordTranslations = function (url) {
        var _this = this;
        return this.auth.AuthGet(url)
            .map(function (r) { return _this.extractData(r, Mapper.MapTranslations); })
            .catch(this.handleError);
    };
    DictionaryService.prototype.createWordTranslation = function (url, translation) {
        return this.auth.AuthPost(url, translation)
            .catch(this.handleError);
    };
    DictionaryService.prototype.updateWordTranslation = function (url, translation) {
        return this.auth.AuthPut(url, translation)
            .catch(this.handleError);
    };
    DictionaryService.prototype.deleteWordTranslation = function (deleteLink) {
        return this.auth.AuthDelete(deleteLink)
            .catch(this.handleError);
    };
    DictionaryService.prototype.getMeanings = function (url) {
        var _this = this;
        return this.auth.AuthGet(url)
            .map(function (r) { return _this.extractData(r, Mapper.MapMeanings); })
            .catch(this.handleError);
    };
    DictionaryService.prototype.createMeaning = function (url, meaning) {
        return this.auth.AuthPost(url, meaning)
            .catch(this.handleError);
    };
    DictionaryService.prototype.updateMeaning = function (url, meaning) {
        return this.auth.AuthPut(url, meaning)
            .catch(this.handleError);
    };
    DictionaryService.prototype.deleteMeaning = function (deleteLink) {
        return this.auth.AuthDelete(deleteLink)
            .catch(this.handleError);
    };
    DictionaryService.prototype.getWordDetails = function (url) {
        var _this = this;
        return this.auth.AuthGet(url)
            .map(function (r) { return _this.extractData(r, Mapper.MapWordDetails); })
            .catch(this.handleError);
    };
    DictionaryService.prototype.createWordDetail = function (url, wordDetail) {
        return this.auth.AuthPost(url, wordDetail)
            .catch(this.handleError);
    };
    DictionaryService.prototype.updateWordDetail = function (url, wordDetail) {
        return this.auth.AuthPut(url, wordDetail)
            .catch(this.handleError);
    };
    DictionaryService.prototype.deleteWordDetail = function (deleteLink) {
        return this.auth.AuthDelete(deleteLink)
            .catch(this.handleError);
    };
    DictionaryService.prototype.createRelation = function (url, relation) {
        return this.auth.AuthPost(url, relation)
            .catch(this.handleError);
    };
    DictionaryService.prototype.updateRelation = function (url, relation) {
        return this.auth.AuthPut(url, relation)
            .catch(this.handleError);
    };
    DictionaryService.prototype.deleteRelation = function (deleteLink) {
        return this.auth.AuthDelete(deleteLink)
            .catch(this.handleError);
    };
    DictionaryService.prototype.extractData = function (res, converter) {
        var body = res.json();
        return converter(body);
    };
    DictionaryService.prototype.handleError = function (error) {
        var errMsg = (error.message) ? error.message :
            error.status ? error.status + " - " + error.statusText : 'Server error';
        console.error(errMsg); // log to console instead
        if (error.stack)
            console.error(error.stack);
        return Observable.throw(errMsg);
    };
    return DictionaryService;
}());
DictionaryService = DictionaryService_1 = __decorate([
    Injectable(),
    __metadata("design:paramtypes", [AuthService,
        Http])
], DictionaryService);
export { DictionaryService };
var DictionaryService_1;
//# sourceMappingURL=dictionary.service.js.map