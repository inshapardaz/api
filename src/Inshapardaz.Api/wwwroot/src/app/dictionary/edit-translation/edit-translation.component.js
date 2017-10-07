var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { DictionaryService } from '../../../services/dictionary.service';
import { Translation } from '../../../models/Translation';
import { Languages } from '../../../models/language';
import { AlertService } from '../../../services/alert.service';
var EditWordTranslationComponent = (function () {
    function EditWordTranslationComponent(dictionaryService, router, translate, alertService) {
        this.dictionaryService = dictionaryService;
        this.router = router;
        this.translate = translate;
        this.alertService = alertService;
        this.model = new Translation();
        this.languagesEnum = Languages;
        this._visible = false;
        this.isBusy = false;
        this.isCreating = false;
        this.createLink = '';
        this.modalId = '';
        this.translation = null;
        this.onClosed = new EventEmitter();
        this.languages = Object.keys(this.languagesEnum).filter(Number);
    }
    Object.defineProperty(EditWordTranslationComponent.prototype, "visible", {
        get: function () { return this._visible; },
        set: function (isVisible) {
            this._visible = isVisible;
            this.isBusy = false;
            if (isVisible) {
                if (this.translation == null) {
                    this.model = new Translation();
                    this.isCreating = true;
                }
                else {
                    this.model = Object.assign({}, this.translation);
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
    EditWordTranslationComponent.prototype.onSubmit = function () {
        var _this = this;
        this.isBusy = false;
        if (this.isCreating) {
            this.dictionaryService.createWordTranslation(this.createLink, this.model)
                .subscribe(function (m) {
                _this.isBusy = false;
                _this.onClosed.emit(true);
                _this.visible = false;
                _this.alertService.success(_this.translate.instant('WORDTRANSLATION.MESSAGES.CREATION_SUCCESS'));
            }, function (error) {
                _this.isBusy = false;
                _this.alertService.error(_this.translate.instant('WORDTRANSLATION.MESSAGES.CREATION_FAILURE'));
            });
        }
        else {
            this.dictionaryService.updateWordTranslation(this.model.updateLink, this.model)
                .subscribe(function (m) {
                _this.isBusy = false;
                _this.onClosed.emit(true);
                _this.visible = false;
                _this.alertService.success(_this.translate.instant('WORDTRANSLATION.MESSAGES.UPDATE_SUCCESS'));
            }, function (error) {
                _this.isBusy = false;
                _this.alertService.error(_this.translate.instant('WORDTRANSLATION.MESSAGES.UPDATE_FAILURE'));
            });
        }
    };
    EditWordTranslationComponent.prototype.onClose = function () {
        this.onClosed.emit(false);
        this.visible = false;
    };
    return EditWordTranslationComponent;
}());
__decorate([
    Input(),
    __metadata("design:type", String)
], EditWordTranslationComponent.prototype, "createLink", void 0);
__decorate([
    Input(),
    __metadata("design:type", String)
], EditWordTranslationComponent.prototype, "modalId", void 0);
__decorate([
    Input(),
    __metadata("design:type", Translation)
], EditWordTranslationComponent.prototype, "translation", void 0);
__decorate([
    Output(),
    __metadata("design:type", Object)
], EditWordTranslationComponent.prototype, "onClosed", void 0);
__decorate([
    Input(),
    __metadata("design:type", Boolean),
    __metadata("design:paramtypes", [Boolean])
], EditWordTranslationComponent.prototype, "visible", null);
EditWordTranslationComponent = __decorate([
    Component({
        selector: 'edit-translation',
        templateUrl: './edit-translation.html'
    }),
    __metadata("design:paramtypes", [DictionaryService,
        Router,
        TranslateService,
        AlertService])
], EditWordTranslationComponent);
export { EditWordTranslationComponent };
//# sourceMappingURL=edit-translation.component.js.map