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
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { DictionaryService } from '../../../services/dictionary.service';
import { Meaning } from '../../../models/Meaning';
import { Languages } from '../../../models/language';
var EditMeaningComponent = (function () {
    function EditMeaningComponent(dictionaryService, router, translate, alertService) {
        this.dictionaryService = dictionaryService;
        this.router = router;
        this.translate = translate;
        this.alertService = alertService;
        this.model = new Meaning();
        this.languagesEnum = Languages;
        this._visible = false;
        this.isBusy = false;
        this.isCreating = false;
        this.createLink = '';
        this.modalId = '';
        this.meaning = null;
        this.onClosed = new EventEmitter();
        this.languages = Object.keys(this.languagesEnum).filter(Number);
    }
    Object.defineProperty(EditMeaningComponent.prototype, "visible", {
        get: function () { return this._visible; },
        set: function (isVisible) {
            this._visible = isVisible;
            this.isBusy = false;
            if (isVisible) {
                if (this.meaning == null) {
                    this.model = new Meaning();
                    this.isCreating = true;
                }
                else {
                    this.model = Object.assign({}, this.meaning);
                    this.isCreating = false;
                }
                $('#' + this.modalId).modal('show');
            }
            else {
                $('#' + this.modalId).modal('hide');
            }
        },
        enumerable: true,
        configurable: true
    });
    EditMeaningComponent.prototype.onSubmit = function () {
        var _this = this;
        this.isBusy = false;
        if (this.isCreating) {
            this.dictionaryService.createMeaning(this.createLink, this.model)
                .subscribe(function (m) {
                _this.isBusy = false;
                _this.onClosed.emit(true);
                _this.visible = false;
                _this.alertService.success(_this.translate.instant('MEANING.MESSAGES.CREATION_SUCCESS'));
            }, function (error) {
                _this.isBusy = false;
                _this.alertService.error(_this.translate.instant('MEANING.MESSAGES.CREATION_FAILURE'));
            });
        }
        else {
            this.dictionaryService.updateMeaning(this.model.updateLink, this.model)
                .subscribe(function (m) {
                _this.isBusy = false;
                _this.onClosed.emit(true);
                _this.visible = false;
                _this.alertService.success(_this.translate.instant('MEANING.MESSAGES.UPDATE_SUCCESS'));
            }, function (error) {
                _this.isBusy = false;
                _this.alertService.error(_this.translate.instant('MEANING.MESSAGES.UPDATE_FAILURE'));
            });
        }
    };
    EditMeaningComponent.prototype.onClose = function () {
        this.visible = false;
        this.onClosed.emit(false);
    };
    return EditMeaningComponent;
}());
__decorate([
    Input(),
    __metadata("design:type", String)
], EditMeaningComponent.prototype, "createLink", void 0);
__decorate([
    Input(),
    __metadata("design:type", String)
], EditMeaningComponent.prototype, "modalId", void 0);
__decorate([
    Input(),
    __metadata("design:type", Meaning)
], EditMeaningComponent.prototype, "meaning", void 0);
__decorate([
    Output(),
    __metadata("design:type", Object)
], EditMeaningComponent.prototype, "onClosed", void 0);
__decorate([
    Input(),
    __metadata("design:type", Boolean),
    __metadata("design:paramtypes", [Boolean])
], EditMeaningComponent.prototype, "visible", null);
EditMeaningComponent = __decorate([
    Component({
        selector: 'edit-meaning',
        templateUrl: './edit-meaning.html'
    }),
    __metadata("design:paramtypes", [DictionaryService,
        Router,
        TranslateService,
        AlertService])
], EditMeaningComponent);
export { EditMeaningComponent };
//# sourceMappingURL=edit-meaning.component.js.map