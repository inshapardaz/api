var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { Languages } from '../../../models/language';
import { DictionaryService } from '../../../services/dictionary.service';
import { AlertService } from '../../../services/alert.service';
var Index = (function () {
    function Index() {
    }
    return Index;
}());
export { Index };
var DictionaryByLinkComponent = (function () {
    function DictionaryByLinkComponent(route, router) {
        this.route = route;
        this.router = router;
    }
    DictionaryByLinkComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.sub = this.route.params.subscribe(function (params) {
            var dictionaryLink = params['link'];
            if (dictionaryLink === '' || dictionaryLink == null) {
                _this.router.navigate(['dictionaries']);
            }
            else {
                var id = dictionaryLink.substring(dictionaryLink.lastIndexOf('/') + 1);
                _this.router.navigate(['dictionary', id]);
            }
        });
    };
    DictionaryByLinkComponent.prototype.ngOnDestroy = function () {
        this.sub.unsubscribe();
    };
    return DictionaryByLinkComponent;
}());
DictionaryByLinkComponent = __decorate([
    Component({
        selector: 'dictionary',
        templateUrl: '../empty.html'
    }),
    __metadata("design:paramtypes", [ActivatedRoute,
        Router])
], DictionaryByLinkComponent);
export { DictionaryByLinkComponent };
var DictionaryComponent = (function () {
    function DictionaryComponent(route, router, fb, alertService, translate, dictionaryService) {
        this.route = route;
        this.router = router;
        this.fb = fb;
        this.alertService = alertService;
        this.translate = translate;
        this.dictionaryService = dictionaryService;
        this.isInSearch = false;
        this.isLoading = false;
        this.pageNumber = 0;
        this.selectedWord = null;
        this.showCreateDialog = false;
        this.Languages = Languages;
        this.searchForm = this.fb.group({
            query: [""]
        });
    }
    DictionaryComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.sub = this.route.params.subscribe(function (params) {
            _this.id = params['id'];
            _this.pageNumber = params['page'] || 1;
            if (_this.dictionary == null) {
                _this.getDictionary(function (d) {
                    _this.getWords(d.indexLink);
                });
            }
            else {
                _this.getWords(_this.dictionary.indexLink);
            }
        });
        //Subscribe to search query parameter
        this.searchText = this.route.queryParams
            .map(function (params) { return params['search'] || ''; });
        this.searchText.subscribe(function (val) {
            if (val != null && val !== "") {
                _this.searchForm.controls.query.setValue(val);
                if (_this.dictionary == null) {
                    _this.getDictionary(function (d) {
                        _this.doSearch();
                    });
                }
                else {
                    _this.doSearch();
                }
            }
        });
        // Subscribe to startWith query parameter
        this.selectedIndex = this.route.queryParams
            .map(function (params) { return params['startWith'] || ''; });
        this.selectedIndex.subscribe(function (val) {
            if (val !== "")
                _this.getIndex(val);
        });
    };
    DictionaryComponent.prototype.ngOnDestroy = function () {
        this.sub.unsubscribe();
    };
    DictionaryComponent.prototype.getIndex = function (index) {
        var _this = this;
        this.isInSearch = false;
        this.isLoading = true;
        this.dictionaryService.getWordStartingWith(index, this.pageNumber)
            .subscribe(function (words) {
            _this.wordPage = words;
            _this.isLoading = false;
        }, function (error) {
            _this.isLoading = false;
            _this.alertService.error(_this.translate.instant('DICTIONARY.MESSAGES.LOADING_FAILURE'));
            _this.errorMessage = error;
        });
    };
    DictionaryComponent.prototype.getDictionary = function (callback) {
        var _this = this;
        this.isLoading = true;
        this.dictionaryService.getDictionary(this.id)
            .subscribe(function (dict) {
            _this.dictionary = dict;
            _this.isLoading = false;
            _this.createWordLink = dict.createWordLink;
            callback(dict);
        }, function (error) {
            _this.isLoading = false;
            _this.alertService.error(_this.translate.instant('DICTIONARY.MESSAGES.LOADING_FAILURE'));
            _this.errorMessage = error;
        });
    };
    DictionaryComponent.prototype.getWords = function (link) {
        var _this = this;
        this.isInSearch = false;
        this.isLoading = true;
        this.dictionaryService.getWords(link, this.pageNumber)
            .subscribe(function (words) {
            _this.wordPage = words;
            _this.isLoading = false;
            _this.loadedLink = link;
        }, function (error) {
            _this.isLoading = false;
            _this.alertService.error(_this.translate.instant('DICTIONARY.MESSAGES.WORDS_LOADING_FAILURE'));
            _this.errorMessage = error;
        });
    };
    DictionaryComponent.prototype.goNext = function () {
        this.pageNumber++;
        this.navigateToPage();
    };
    DictionaryComponent.prototype.goPrevious = function () {
        this.pageNumber--;
        this.navigateToPage();
    };
    DictionaryComponent.prototype.gotoIndex = function (index) {
        var navigationExtras = {
            queryParams: { 'startWith': index.link },
        };
        this.router.navigate(['/dictionary', this.id], navigationExtras);
    };
    DictionaryComponent.prototype.gotoSearch = function () {
        var query = this.searchForm.controls.query.value;
        if (query == null || query.length < 0)
            return;
        var navigationExtras = {
            queryParams: { 'search': query }
        };
        this.router.navigate(['/dictionary', this.id], navigationExtras);
    };
    DictionaryComponent.prototype.navigateToPage = function () {
        var startWith = this.route.snapshot.queryParams["startWith"];
        var search = this.route.snapshot.queryParams["search"];
        if (startWith != null && startWith != '') {
            var navigationExtras = {
                queryParams: { 'startWith': startWith }
            };
            this.router.navigate(['dictionary', this.id, this.pageNumber], navigationExtras);
        }
        else if (search != null && search != '') {
            var navigationExtras = {
                queryParams: { 'search': search }
            };
            this.router.navigate(['dictionary', this.id, this.pageNumber], navigationExtras);
        }
        else {
            this.router.navigate(['dictionary', this.id, this.pageNumber]);
        }
    };
    DictionaryComponent.prototype.reloadPage = function () {
        this.getWords(this.loadedLink);
    };
    DictionaryComponent.prototype.clearSearch = function () {
        this.searchForm.setValue({ query: '' });
        this.isInSearch = false;
        this.reloadPage();
    };
    DictionaryComponent.prototype.doSearch = function () {
        var _this = this;
        var searchValue = this.searchForm.controls.query.value;
        if (searchValue != null && searchValue.length > 0) {
            this.isInSearch = true;
            this.isLoading = true;
            this.dictionaryService.searchWords(this.dictionary.searchLink, searchValue, this.pageNumber)
                .subscribe(function (words) {
                _this.wordPage = words;
                _this.isLoading = false;
            }, function (error) {
                _this.isLoading = false;
                _this.alertService.error(_this.translate.instant('DICTIONARY.MESSAGES.SEARCH_LOADING_FAILURE'));
                _this.errorMessage = error;
            });
        }
    };
    DictionaryComponent.prototype.addWord = function () {
        this.showCreateDialog = true;
    };
    DictionaryComponent.prototype.onCreateClosed = function (created) {
        this.showCreateDialog = false;
        this.selectedWord = null;
        if (created) {
            this.reloadPage();
        }
    };
    DictionaryComponent.prototype.editWord = function (word) {
        this.showCreateDialog = true;
        this.selectedWord = word;
    };
    DictionaryComponent.prototype.deleteWord = function (word) {
        var _this = this;
        this.isLoading = true;
        this.dictionaryService.deleteWord(word.deleteLink)
            .subscribe(function (r) {
            _this.alertService.success(_this.translate.instant('WORD.MESSAGES.DELETE_SUCCESS', { title: word.title }));
            _this.reloadPage();
        }, function (e) {
            _this.errorMessage = e;
            _this.isLoading = false;
            _this.alertService.error(_this.translate.instant('WORD.MESSAGES.DELETE_FAILURE', { title: word.title }));
        });
    };
    return DictionaryComponent;
}());
DictionaryComponent = __decorate([
    Component({
        selector: 'dictionary',
        templateUrl: './dictionary.component.html'
    }),
    __metadata("design:paramtypes", [ActivatedRoute,
        Router,
        FormBuilder,
        AlertService,
        TranslateService,
        DictionaryService])
], DictionaryComponent);
export { DictionaryComponent };
//# sourceMappingURL=dictionary.component.js.map