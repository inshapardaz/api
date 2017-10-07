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
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Languages } from '../../../models/language';
import { DictionaryService } from '../../../services/dictionary.service';
import { AuthService } from '../../../services/auth.service';
import { AlertService } from '../../../services/alert.service';
var DictionariesComponent = (function () {
    function DictionariesComponent(dictionaryService, auth, alertService, router, translate) {
        this.dictionaryService = dictionaryService;
        this.auth = auth;
        this.alertService = alertService;
        this.router = router;
        this.translate = translate;
        this.isLoading = false;
        this.errorLoadingDictionaries = false;
        this.showCreateDialog = false;
        this.Languages = Languages;
    }
    DictionariesComponent.prototype.ngOnInit = function () {
        this.getEntry();
    };
    DictionariesComponent.prototype.deleteDictionary = function (dictionary) {
        var _this = this;
        this.dictionaryService.deleteDictionary(dictionary.deleteLink)
            .subscribe(function (r) {
            _this.alertService.success(_this.translate.instant('DICTIONARIES.MESSAGES.DELETION_SUCCESS', { name: dictionary.name }));
            _this.getDictionaries();
        }, function (e) {
            _this.handlerError();
            _this.alertService.error(_this.translate.instant('DICTIONARIES.MESSAGES.DELETION_FAILURE', { name: dictionary.name }));
        });
    };
    DictionariesComponent.prototype.getEntry = function () {
        var _this = this;
        this.isLoading = true;
        this.dictionaryService.getEntry()
            .subscribe(function (entry) {
            _this.dictionariesLink = entry.dictionariesLink;
            _this.getDictionaries();
        }, function (e) {
            _this.handlerError();
            _this.alertService.error(_this.translate.instant('DICTIONARIES.MESSAGES.LOADING_FAILURE'));
            _this.router.navigate(['/error/servererror']);
        });
    };
    DictionariesComponent.prototype.getDictionaries = function () {
        var _this = this;
        this.errorLoadingDictionaries = false;
        this.dictionaryService.getDictionaries(this.dictionariesLink)
            .subscribe(function (data) {
            _this.dictionaries = data.dictionaries;
            _this.createLink = data.createLink;
            _this.isLoading = false;
        }, function (e) {
            _this.handlerError();
            _this.errorLoadingDictionaries = true;
            _this.alertService.error(_this.translate.instant('DICTIONARIES.MESSAGES.LOADING_FAILURE'));
        });
    };
    DictionariesComponent.prototype.createDictionary = function () {
        this.selectedDictionary = null;
        this.showCreateDialog = true;
    };
    DictionariesComponent.prototype.editDictionary = function (dictionary) {
        this.selectedDictionary = dictionary;
        this.showCreateDialog = true;
    };
    DictionariesComponent.prototype.createDictionaryDownload = function (dictionary) {
        var _this = this;
        this.dictionaryService.createDictionaryDownload(dictionary.createDownloadLink)
            .subscribe(function (data) {
            _this.alertService.success(_this.translate.instant('DICTIONARIES.MESSAGES.CREATION_SUCCESS'));
        }, function (e) {
            _this.handlerError();
            _this.alertService.error(_this.translate.instant('DICTIONARIES.MESSAGES.DOWNLOAD_REQUEST_FAILURE'));
        });
    };
    DictionariesComponent.prototype.onCreateClosed = function (created) {
        this.showCreateDialog = false;
        if (created) {
            this.getDictionaries();
        }
    };
    DictionariesComponent.prototype.handlerError = function () {
        this.isLoading = false;
    };
    return DictionariesComponent;
}());
DictionariesComponent = __decorate([
    Component({
        selector: 'dictionaries',
        templateUrl: './dictionaries.component.html'
    }),
    __metadata("design:paramtypes", [DictionaryService,
        AuthService,
        AlertService,
        Router,
        TranslateService])
], DictionariesComponent);
export { DictionariesComponent };
//# sourceMappingURL=dictionaries.component.js.map