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
import { DictionaryService } from '../../../services/dictionary.service';
import { AlertService } from '../../../services/alert.service';
import { TranslateService } from '@ngx-translate/core';
var WordComponent = (function () {
    function WordComponent(route, router, alertService, translate, dictionaryService) {
        this.route = route;
        this.router = router;
        this.alertService = alertService;
        this.translate = translate;
        this.dictionaryService = dictionaryService;
        this.isBusy = false;
        this.showEditDialog = false;
    }
    WordComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.sub = this.route.params.subscribe(function (params) {
            _this.id = params['id'];
            _this.getWord();
        });
    };
    WordComponent.prototype.ngOnDestroy = function () {
        this.sub.unsubscribe();
    };
    WordComponent.prototype.getWord = function () {
        var _this = this;
        this.isBusy = true;
        this.dictionaryService.getWordById(this.id)
            .subscribe(function (word) {
            _this.word = word;
            _this.isBusy = false;
        }, function (error) {
            _this.isBusy = false;
            _this.alertService.error(_this.translate.instant('WORD.MESSAGES.LOAD_FAILURE'));
            _this.errorMessage = error;
        });
    };
    WordComponent.prototype.editWord = function () {
        this.showEditDialog = true;
    };
    WordComponent.prototype.deleteWord = function () {
        var _this = this;
        this.isBusy = true;
        this.dictionaryService.deleteWord(this.word.deleteLink)
            .subscribe(function (r) {
            _this.isBusy = false;
            _this.alertService.success(_this.translate.instant('WORD.MESSAGES.DELETE_SUCCESS', { title: _this.word.title }));
            _this.router.navigate(['dictionaryLink', _this.word.dictionaryLink]);
        }, function (error) {
            _this.errorMessage = error;
            _this.isBusy = false;
            _this.alertService.error(_this.translate.instant('WORD.MESSAGES.DELETE_FAILURE', { title: _this.word.title }));
        });
    };
    WordComponent.prototype.onEditClosed = function (created) {
        this.showEditDialog = false;
        if (created) {
            this.getWord();
        }
    };
    return WordComponent;
}());
WordComponent = __decorate([
    Component({
        selector: 'word',
        templateUrl: './word.component.html'
    }),
    __metadata("design:paramtypes", [ActivatedRoute,
        Router,
        AlertService,
        TranslateService,
        DictionaryService])
], WordComponent);
export { WordComponent };
//# sourceMappingURL=word.component.js.map