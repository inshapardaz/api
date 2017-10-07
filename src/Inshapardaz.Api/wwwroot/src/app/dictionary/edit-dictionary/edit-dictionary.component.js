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
import { AlertService } from '../../../services/alert.service';
import { Dictionary } from '../../../models/Dictionary';
import { Languages } from '../../../models/language';
var EditDictionaryComponent = (function () {
    function EditDictionaryComponent(dictionaryService, router, translate, alertService) {
        this.dictionaryService = dictionaryService;
        this.router = router;
        this.translate = translate;
        this.alertService = alertService;
        this.model = new Dictionary();
        this.languagesEnum = Languages;
        this._visible = false;
        this.isBusy = false;
        this.isCreating = false;
        this.createLink = '';
        this.modalId = '';
        this.dictionary = null;
        this.onClosed = new EventEmitter();
        this.languages = Object.keys(this.languagesEnum).filter(Number);
        this.model.language = Languages.Urdu;
    }
    Object.defineProperty(EditDictionaryComponent.prototype, "visible", {
        get: function () { return this._visible; },
        set: function (isVisible) {
            this._visible = isVisible;
            this.isBusy = false;
            if (isVisible) {
                if (this.dictionary == null) {
                    this.model = new Dictionary();
                    this.isCreating = true;
                }
                else {
                    this.model = Object.assign({}, this.dictionary);
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
    EditDictionaryComponent.prototype.onSubmit = function () {
        var _this = this;
        if (this.isBusy) {
            return;
        }
        this.isBusy = true;
        if (this.isCreating) {
            this.dictionaryService.createDictionary(this.createLink, this.model)
                .subscribe(function (m) {
                _this.isBusy = false;
                _this.onClosed.emit(true);
                _this.visible = false;
                _this.alertService.success(_this.translate.instant('DICTIONARIES.MESSAGES.CREATION_SUCCESS', { name: _this.model.name }));
            }, function (e) {
                _this.isBusy = false;
                _this.alertService.error(_this.translate.instant('DICTIONARIES.MESSAGES.CREATION_FAILURE', { name: _this.model.name }));
            });
        }
        else {
            this.dictionaryService.updateDictionary(this.model.updateLink, this.model)
                .subscribe(function (m) {
                _this.isBusy = false;
                _this.onClosed.emit(true);
                _this.visible = false;
                _this.alertService.success(_this.translate.instant('DICTIONARIES.MESSAGES.UPDATE_SUCCESS', { name: _this.model.name }));
            }, function (e) {
                _this.isBusy = false;
                _this.alertService.error(_this.translate.instant('DICTIONARIES.MESSAGES.UPDATE_FAILURE', { name: _this.model.name }));
            });
        }
    };
    EditDictionaryComponent.prototype.onClose = function () {
        this.onClosed.emit(false);
        this.visible = false;
    };
    return EditDictionaryComponent;
}());
__decorate([
    Input(),
    __metadata("design:type", String)
], EditDictionaryComponent.prototype, "createLink", void 0);
__decorate([
    Input(),
    __metadata("design:type", String)
], EditDictionaryComponent.prototype, "modalId", void 0);
__decorate([
    Input(),
    __metadata("design:type", Dictionary)
], EditDictionaryComponent.prototype, "dictionary", void 0);
__decorate([
    Output(),
    __metadata("design:type", Object)
], EditDictionaryComponent.prototype, "onClosed", void 0);
__decorate([
    Input(),
    __metadata("design:type", Boolean),
    __metadata("design:paramtypes", [Boolean])
], EditDictionaryComponent.prototype, "visible", null);
EditDictionaryComponent = __decorate([
    Component({
        selector: 'edit-dictionaries',
        templateUrl: './edit-dictionary.html'
    }),
    __metadata("design:paramtypes", [DictionaryService,
        Router,
        TranslateService,
        AlertService])
], EditDictionaryComponent);
export { EditDictionaryComponent };
//# sourceMappingURL=edit-dictionary.component.js.map