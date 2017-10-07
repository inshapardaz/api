var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { AlertService } from '../../../services/alert.service';
import { TranslateService } from '@ngx-translate/core';
import { Component, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { DictionaryService } from '../../../services/dictionary.service';
var WordDetailsComponent = (function () {
    function WordDetailsComponent(route, router, alertService, translate, dictionaryService) {
        this.route = route;
        this.router = router;
        this.alertService = alertService;
        this.translate = translate;
        this.dictionaryService = dictionaryService;
        this.showEditDialog = false;
        this.createLink = '';
        this.isBusy = false;
    }
    Object.defineProperty(WordDetailsComponent.prototype, "wordDetailLink", {
        get: function () { return this._wordDetailLink; },
        set: function (wordDetailLink) {
            this._wordDetailLink = (wordDetailLink) || '';
            this.getWordDetails();
        },
        enumerable: true,
        configurable: true
    });
    WordDetailsComponent.prototype.getWordDetails = function () {
        var _this = this;
        this.isBusy = true;
        this.dictionaryService.getWordDetails(this._wordDetailLink)
            .subscribe(function (details) {
            _this.wordDetails = details;
            _this.isBusy = false;
        }, function (error) {
            _this.isBusy = false;
            _this.alertService.error(_this.translate.instant('WORDDETAIL.MESSAGES.LOAD_FAILURE'));
            _this.errorMessage = error;
        });
    };
    WordDetailsComponent.prototype.editDetail = function (detail) {
        this.selectedDetail = detail;
        this.showEditDialog = true;
    };
    WordDetailsComponent.prototype.deleteDetail = function (detail) {
        var _this = this;
        this.isBusy = true;
        this.dictionaryService.deleteWordDetail(detail.deleteLink)
            .subscribe(function (r) {
            _this.isBusy = false;
            _this.alertService.success(_this.translate.instant('WORDDETAIL.MESSAGES.DELETE_SUCCESS'));
            _this.getWordDetails();
        }, function (error) {
            _this.isBusy = false;
            _this.alertService.error(_this.translate.instant('WORDDETAIL.MESSAGES.DELETE_FAILURE'));
            _this.errorMessage = error;
        });
    };
    WordDetailsComponent.prototype.addDetail = function () {
        this.selectedDetail = null;
        this.showEditDialog = true;
    };
    WordDetailsComponent.prototype.onEditClosed = function (created) {
        this.showEditDialog = false;
        if (created) {
            this.getWordDetails();
        }
    };
    return WordDetailsComponent;
}());
__decorate([
    Input(),
    __metadata("design:type", String)
], WordDetailsComponent.prototype, "createLink", void 0);
__decorate([
    Input(),
    __metadata("design:type", String),
    __metadata("design:paramtypes", [String])
], WordDetailsComponent.prototype, "wordDetailLink", null);
WordDetailsComponent = __decorate([
    Component({
        selector: 'word-detail',
        templateUrl: './wordDetail.html'
    }),
    __metadata("design:paramtypes", [ActivatedRoute,
        Router,
        AlertService,
        TranslateService,
        DictionaryService])
], WordDetailsComponent);
export { WordDetailsComponent };
//# sourceMappingURL=wordDetail.component.js.map