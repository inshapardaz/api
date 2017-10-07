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
import { Word } from '../../../models/Word';
import { Languages } from '../../../models/language';
import { AlertService } from '../../../services/alert.service';
var EditWordComponent = (function () {
    function EditWordComponent(dictionaryService, router, translate, alertService) {
        this.dictionaryService = dictionaryService;
        this.router = router;
        this.translate = translate;
        this.alertService = alertService;
        this.model = new Word();
        this.languagesEnum = Languages;
        this._visible = false;
        this.isBusy = false;
        this.isCreating = false;
        this.createLink = '';
        this.modalId = '';
        this.word = null;
        this.onClosed = new EventEmitter();
        this.languages = Object.keys(this.languagesEnum).filter(Number);
    }
    Object.defineProperty(EditWordComponent.prototype, "visible", {
        get: function () { return this._visible; },
        set: function (isVisible) {
            this._visible = isVisible;
            this.isBusy = false;
            if (isVisible) {
                if (this.word == null) {
                    this.model = new Word();
                    this.isCreating = true;
                }
                else {
                    this.model = Object.assign({}, this.word);
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
    EditWordComponent.prototype.onSubmit = function () {
        var _this = this;
        this.isBusy = true;
        if (this.isCreating) {
            this.dictionaryService.createWord(this.createLink, this.model)
                .subscribe(function (m) {
                _this.isBusy = false;
                _this.onClosed.emit(true);
                _this.visible = false;
                _this.alertService.success(_this.translate.instant('WORD.MESSAGES.CREATION_SUCCESS', { title: _this.model.title }));
            }, function (e) {
                _this.isBusy = false;
                _this.alertService.error(_this.translate.instant('WORD.MESSAGES.CREATION_FAILURE', { title: _this.model.title }));
            });
        }
        else {
            this.dictionaryService.updateWord(this.model.updateLink, this.model)
                .subscribe(function (m) {
                _this.isBusy = false;
                _this.onClosed.emit(true);
                _this.visible = false;
                _this.alertService.success(_this.translate.instant('WORD.MESSAGES.UPDATE_SUCCESS', { title: _this.model.title }));
            }, function (e) {
                _this.isBusy = false;
                _this.alertService.error(_this.translate.instant('WORD.MESSAGES.UPDATE_FAILURE', { title: _this.model.title }));
            });
        }
    };
    EditWordComponent.prototype.onClose = function () {
        this.onClosed.emit(false);
        this.visible = false;
    };
    return EditWordComponent;
}());
__decorate([
    Input(),
    __metadata("design:type", String)
], EditWordComponent.prototype, "createLink", void 0);
__decorate([
    Input(),
    __metadata("design:type", String)
], EditWordComponent.prototype, "modalId", void 0);
__decorate([
    Input(),
    __metadata("design:type", Word)
], EditWordComponent.prototype, "word", void 0);
__decorate([
    Output(),
    __metadata("design:type", Object)
], EditWordComponent.prototype, "onClosed", void 0);
__decorate([
    Input(),
    __metadata("design:type", Boolean),
    __metadata("design:paramtypes", [Boolean])
], EditWordComponent.prototype, "visible", null);
EditWordComponent = __decorate([
    Component({
        selector: 'edit-word',
        templateUrl: './edit-word.html'
    }),
    __metadata("design:paramtypes", [DictionaryService,
        Router,
        TranslateService,
        AlertService])
], EditWordComponent);
export { EditWordComponent };
//# sourceMappingURL=edit-word.component.js.map