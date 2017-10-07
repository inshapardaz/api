var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { TranslateService } from '@ngx-translate/core';
import { AlertService } from '../../../services/alert.service';
import { Component, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { DictionaryService } from '../../../services/dictionary.service';
var WordTranslationsComponent = (function () {
    function WordTranslationsComponent(route, router, alertService, translate, dictionaryService) {
        this.route = route;
        this.router = router;
        this.alertService = alertService;
        this.translate = translate;
        this.dictionaryService = dictionaryService;
        this.isLoading = false;
        this.showEditDialog = false;
    }
    Object.defineProperty(WordTranslationsComponent.prototype, "translationsLink", {
        get: function () { return this._translationsLink; },
        set: function (translationLink) {
            this._translationsLink = (translationLink) || '';
            this.getTranslations();
        },
        enumerable: true,
        configurable: true
    });
    WordTranslationsComponent.prototype.getTranslations = function () {
        var _this = this;
        this.isLoading = true;
        this.dictionaryService.getWordTranslations(this._translationsLink)
            .subscribe(function (translations) {
            _this.translations = translations;
            _this.isLoading = false;
        }, function (error) {
            _this.errorMessage = error;
            _this.isLoading = false;
            _this.alertService.error(_this.translate.instant('WORDTRANSLATION.MESSAGES.LOAD_FAILURE'));
        });
    };
    WordTranslationsComponent.prototype.addTranslation = function () {
        this.selectedTranslation = null;
        this.showEditDialog = true;
    };
    WordTranslationsComponent.prototype.editTranslation = function (translation) {
        this.selectedTranslation = translation;
        this.showEditDialog = true;
    };
    WordTranslationsComponent.prototype.deleteTranslation = function (translation) {
        var _this = this;
        this.dictionaryService.deleteWordTranslation(translation.deleteLink)
            .subscribe(function (r) {
            _this.alertService.success(_this.translate.instant('WORDTRANSLATION.MESSAGES.DELETE_SUCCESS'));
            _this.getTranslations();
        }, function (error) {
            _this.errorMessage = error;
            _this.isLoading = false;
            _this.alertService.error(_this.translate.instant('WORDTRANSLATION.MESSAGES.DELETE_FAILURE'));
        });
    };
    WordTranslationsComponent.prototype.onEditClosed = function (created) {
        this.showEditDialog = false;
        if (created) {
            this.getTranslations();
        }
    };
    return WordTranslationsComponent;
}());
__decorate([
    Input(),
    __metadata("design:type", String)
], WordTranslationsComponent.prototype, "createLink", void 0);
__decorate([
    Input(),
    __metadata("design:type", String)
], WordTranslationsComponent.prototype, "wordDetailId", void 0);
__decorate([
    Input(),
    __metadata("design:type", String),
    __metadata("design:paramtypes", [String])
], WordTranslationsComponent.prototype, "translationsLink", null);
WordTranslationsComponent = __decorate([
    Component({
        selector: 'word-translations',
        templateUrl: './Translations.html'
    }),
    __metadata("design:paramtypes", [ActivatedRoute,
        Router,
        AlertService,
        TranslateService,
        DictionaryService])
], WordTranslationsComponent);
export { WordTranslationsComponent };
//# sourceMappingURL=translations.component.js.map