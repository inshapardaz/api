var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Observable } from 'rxjs/Rx';
import { DomSanitizer } from "@angular/platform-browser";
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { DictionaryService } from '../../../services/dictionary.service';
import { Relation } from '../../../models/relation';
import { Languages } from '../../../models/language';
import { Word } from '../../../models/Word';
import { RelationTypes } from '../../../models/relationTypes';
import { AlertService } from '../../../services/alert.service';
var EditWordRelationComponent = (function () {
    function EditWordRelationComponent(dictionaryService, router, translate, alertService, _sanitizer) {
        var _this = this;
        this.dictionaryService = dictionaryService;
        this.router = router;
        this.translate = translate;
        this.alertService = alertService;
        this._sanitizer = _sanitizer;
        this.model = new Relation();
        this.languagesEnum = Languages;
        this.relationTypesEnum = RelationTypes;
        this._visible = false;
        this.isBusy = false;
        this.isCreating = false;
        this.createLink = '';
        this.dictionaryLink = '';
        this.modalId = '';
        this.relation = null;
        this.sourceWord = null;
        this.onClosed = new EventEmitter();
        this.observableSource = function (keyword) {
            if (keyword) {
                return _this.dictionaryService.getWordsStartingWith(_this.dictionaryLink, keyword);
            }
            else {
                return Observable.of([]);
            }
        };
        this.autocompleteListFormatter = function (data) {
            return _this._sanitizer.bypassSecurityTrustHtml(data.title);
        };
        this.languages = Object.keys(this.languagesEnum).filter(Number);
        this.relationTypesValues = Object.keys(this.relationTypesEnum).filter(Number);
    }
    Object.defineProperty(EditWordRelationComponent.prototype, "visible", {
        get: function () { return this._visible; },
        set: function (isVisible) {
            this._visible = isVisible;
            this.isBusy = false;
            if (isVisible) {
                if (this.relation == null) {
                    this.model = new Relation();
                    this.model.sourceWordId = this.sourceWord.id;
                    this.isCreating = true;
                }
                else {
                    this.model = Object.assign({}, this.relation);
                    this.model.sourceWordId = this.sourceWord.id;
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
    EditWordRelationComponent.prototype.relatedWordChanged = function (e) {
        this.model.relatedWordId = e.id;
        this.model.relatedWord = e.title;
    };
    EditWordRelationComponent.prototype.onSubmit = function () {
        var _this = this;
        this.isBusy = false;
        if (this.isCreating) {
            this.model.sourceWord = this.sourceWord.title;
            this.dictionaryService.createRelation(this.createLink, this.model)
                .subscribe(function (m) {
                _this.isBusy = false;
                _this.onClosed.emit(true);
                _this.visible = false;
                _this.alertService.success(_this.translate.instant('RELATION.MESSAGES.CREATION_SUCCESS'));
            }, function (error) {
                _this.isBusy = false;
                _this.alertService.error(_this.translate.instant('RELATION.MESSAGES.CREATION_FAILURE'));
            });
        }
        else {
            this.dictionaryService.updateRelation(this.model.updateLink, this.model)
                .subscribe(function (m) {
                _this.isBusy = false;
                _this.onClosed.emit(true);
                _this.visible = false;
                _this.alertService.success(_this.translate.instant('RELATION.MESSAGES.UPDATE_SUCCESS'));
            }, function (error) {
                _this.isBusy = false;
                _this.alertService.error(_this.translate.instant('RELATION.MESSAGES.UPDATE_FAILURE'));
            });
        }
    };
    EditWordRelationComponent.prototype.onClose = function () {
        this.visible = false;
        this.onClosed.emit(false);
    };
    return EditWordRelationComponent;
}());
__decorate([
    Input(),
    __metadata("design:type", String)
], EditWordRelationComponent.prototype, "createLink", void 0);
__decorate([
    Input(),
    __metadata("design:type", String)
], EditWordRelationComponent.prototype, "dictionaryLink", void 0);
__decorate([
    Input(),
    __metadata("design:type", String)
], EditWordRelationComponent.prototype, "modalId", void 0);
__decorate([
    Input(),
    __metadata("design:type", Relation)
], EditWordRelationComponent.prototype, "relation", void 0);
__decorate([
    Input(),
    __metadata("design:type", Word)
], EditWordRelationComponent.prototype, "sourceWord", void 0);
__decorate([
    Output(),
    __metadata("design:type", Object)
], EditWordRelationComponent.prototype, "onClosed", void 0);
__decorate([
    Input(),
    __metadata("design:type", Boolean),
    __metadata("design:paramtypes", [Boolean])
], EditWordRelationComponent.prototype, "visible", null);
EditWordRelationComponent = __decorate([
    Component({
        selector: 'edit-wordRelation',
        templateUrl: './edit-wordRelation.html'
    }),
    __metadata("design:paramtypes", [DictionaryService,
        Router,
        TranslateService,
        AlertService,
        DomSanitizer])
], EditWordRelationComponent);
export { EditWordRelationComponent };
//# sourceMappingURL=edit-wordRelation.component.js.map