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
import { WordDetail } from '../../../models/WordDetail';
import { Languages } from '../../../models/language';
import { GrammaticTypes } from '../../../models/grammaticalTypes';
import { AlertService } from '../../../services/alert.service';
var EditWordDetailComponent = (function () {
    function EditWordDetailComponent(dictionaryService, router, alertService, translate) {
        this.dictionaryService = dictionaryService;
        this.router = router;
        this.alertService = alertService;
        this.translate = translate;
        this.model = new WordDetail();
        this.languagesEnum = Languages;
        this.attributeEnum = GrammaticTypes;
        this._visible = false;
        this.isBusy = false;
        this.isCreating = false;
        this.createLink = '';
        this.modalId = '';
        this.wordDetail = null;
        this.onClosed = new EventEmitter();
        this.languages = Object.keys(this.languagesEnum).filter(Number);
        this.attributesValues = Object.keys(this.attributeEnum).filter(Number);
    }
    Object.defineProperty(EditWordDetailComponent.prototype, "visible", {
        get: function () { return this._visible; },
        set: function (isVisible) {
            this._visible = isVisible;
            this.isBusy = false;
            if (isVisible) {
                if (this.wordDetail == null) {
                    this.model = new WordDetail();
                    this.isCreating = true;
                }
                else {
                    this.model = Object.assign({}, this.wordDetail);
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
    EditWordDetailComponent.prototype.onSubmit = function () {
        var _this = this;
        this.isBusy = false;
        if (this.isCreating) {
            this.dictionaryService.createWordDetail(this.createLink, this.model)
                .subscribe(function (m) {
                _this.isBusy = false;
                _this.onClosed.emit(true);
                _this.visible = false;
                _this.alertService.success(_this.translate.instant('WORDDETAIL.MESSAGES.CREATION_SUCCESS'));
            }, function (error) {
                _this.alertService.error(_this.translate.instant('WORDDETAIL.MESSAGES.CREATION_FAILURE'));
                _this.isBusy = false;
            });
        }
        else {
            this.dictionaryService.updateWordDetail(this.model.updateLink, this.model)
                .subscribe(function (m) {
                _this.isBusy = false;
                _this.onClosed.emit(true);
                _this.visible = false;
                _this.alertService.success(_this.translate.instant('WORDDETAIL.MESSAGES.UPDATE_SUCCESS'));
            }, function (error) {
                _this.alertService.error(_this.translate.instant('WORDDETAIL.MESSAGES.UPDATE_FAILURE'));
                _this.isBusy = false;
            });
        }
    };
    EditWordDetailComponent.prototype.onClose = function () {
        this.onClosed.emit(false);
        this.visible = false;
    };
    return EditWordDetailComponent;
}());
__decorate([
    Input(),
    __metadata("design:type", String)
], EditWordDetailComponent.prototype, "createLink", void 0);
__decorate([
    Input(),
    __metadata("design:type", String)
], EditWordDetailComponent.prototype, "modalId", void 0);
__decorate([
    Input(),
    __metadata("design:type", WordDetail)
], EditWordDetailComponent.prototype, "wordDetail", void 0);
__decorate([
    Output(),
    __metadata("design:type", Object)
], EditWordDetailComponent.prototype, "onClosed", void 0);
__decorate([
    Input(),
    __metadata("design:type", Boolean),
    __metadata("design:paramtypes", [Boolean])
], EditWordDetailComponent.prototype, "visible", null);
EditWordDetailComponent = __decorate([
    Component({
        selector: 'edit-wordDetail',
        templateUrl: './edit-wordDetail.html'
    }),
    __metadata("design:paramtypes", [DictionaryService,
        Router,
        AlertService,
        TranslateService])
], EditWordDetailComponent);
export { EditWordDetailComponent };
//# sourceMappingURL=edit-wordDetail.component.js.map