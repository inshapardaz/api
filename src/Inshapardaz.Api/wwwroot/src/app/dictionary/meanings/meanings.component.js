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
var MeaningsComponent = (function () {
    function MeaningsComponent(route, router, alertService, translate, dictionaryService) {
        this.route = route;
        this.router = router;
        this.alertService = alertService;
        this.translate = translate;
        this.dictionaryService = dictionaryService;
        this.isLoading = false;
        this.showEditDialog = false;
        this.selectedMeaning = null;
    }
    Object.defineProperty(MeaningsComponent.prototype, "meaningsLink", {
        get: function () { return this._meaningsLink; },
        set: function (relationsLink) {
            this._meaningsLink = (relationsLink) || '';
            this.getMeanings();
        },
        enumerable: true,
        configurable: true
    });
    MeaningsComponent.prototype.getMeanings = function () {
        var _this = this;
        this.isLoading = true;
        this.dictionaryService.getMeanings(this._meaningsLink)
            .subscribe(function (meanings) {
            _this.meanings = meanings;
            _this.isLoading = false;
        }, function (error) {
            _this.alertService.error(_this.translate.instant('MEANING.MESSAGES.LOAD_FAILURE'));
            _this.errorMessage = error;
        });
    };
    MeaningsComponent.prototype.addMeaning = function () {
        this.selectedMeaning = null;
        this.showEditDialog = true;
    };
    MeaningsComponent.prototype.editMeaning = function (meaning) {
        this.selectedMeaning = meaning;
        this.showEditDialog = true;
    };
    MeaningsComponent.prototype.deleteMeaning = function (meaning) {
        var _this = this;
        this.dictionaryService.deleteMeaning(meaning.deleteLink)
            .subscribe(function (r) {
            _this.alertService.success(_this.translate.instant('MEANING.MESSAGES.DELETE_SUCCESS'));
            _this.getMeanings();
        }, function (error) {
            _this.errorMessage = error;
            _this.isLoading = false;
            _this.alertService.error(_this.translate.instant('MEANING.MESSAGES.DELETE_FAILURE'));
        });
    };
    MeaningsComponent.prototype.onEditClosed = function (created) {
        this.showEditDialog = false;
        if (created) {
            this.getMeanings();
        }
    };
    return MeaningsComponent;
}());
__decorate([
    Input(),
    __metadata("design:type", String)
], MeaningsComponent.prototype, "createLink", void 0);
__decorate([
    Input(),
    __metadata("design:type", Number)
], MeaningsComponent.prototype, "wordDetailId", void 0);
__decorate([
    Input(),
    __metadata("design:type", String),
    __metadata("design:paramtypes", [String])
], MeaningsComponent.prototype, "meaningsLink", null);
MeaningsComponent = __decorate([
    Component({
        selector: 'word-meanings',
        templateUrl: './Meanings.html'
    }),
    __metadata("design:paramtypes", [ActivatedRoute,
        Router,
        AlertService,
        TranslateService,
        DictionaryService])
], MeaningsComponent);
export { MeaningsComponent };
//# sourceMappingURL=meanings.component.js.map