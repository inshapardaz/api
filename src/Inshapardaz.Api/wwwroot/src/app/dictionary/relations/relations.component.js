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
import { Word } from '../../../models/Word';
var RelationsComponent = (function () {
    function RelationsComponent(route, router, alertService, translate, dictionaryService) {
        this.route = route;
        this.router = router;
        this.alertService = alertService;
        this.translate = translate;
        this.dictionaryService = dictionaryService;
        this.isLoading = false;
        this.selectedRelation = null;
        this.showEditDialog = false;
    }
    Object.defineProperty(RelationsComponent.prototype, "relationsLink", {
        get: function () { return this._relationsLink; },
        set: function (relationsLink) {
            this._relationsLink = (relationsLink) || '';
            this.getRelations();
        },
        enumerable: true,
        configurable: true
    });
    RelationsComponent.prototype.getRelations = function () {
        var _this = this;
        this.isLoading = true;
        this.dictionaryService.getWordRelations(this._relationsLink)
            .subscribe(function (relations) {
            _this.relations = relations;
            _this.isLoading = false;
        }, function (error) {
            _this.alertService.error(_this.translate.instant('RELATION.MESSAGES.LOAD_FAILURE'));
            _this.errorMessage = error;
        });
    };
    RelationsComponent.prototype.addRelation = function () {
        this.selectedRelation = null;
        this.showEditDialog = true;
    };
    RelationsComponent.prototype.editRelation = function (relation) {
        this.selectedRelation = relation;
        this.showEditDialog = true;
    };
    RelationsComponent.prototype.deleteRelation = function (relation) {
        var _this = this;
        this.dictionaryService.deleteRelation(relation.deleteLink)
            .subscribe(function (r) {
            _this.alertService.success(_this.translate.instant('RELATION.MESSAGES.DELETE_SUCCESS'));
            _this.getRelations();
        }, function (error) {
            _this.errorMessage = error;
            _this.alertService.error(_this.translate.instant('RELATION.MESSAGES.DELETE_FAILURE'));
        });
    };
    RelationsComponent.prototype.onEditClosed = function (created) {
        this.showEditDialog = false;
        if (created) {
            this.getRelations();
        }
    };
    return RelationsComponent;
}());
__decorate([
    Input(),
    __metadata("design:type", String)
], RelationsComponent.prototype, "createRelationLink", void 0);
__decorate([
    Input(),
    __metadata("design:type", String)
], RelationsComponent.prototype, "dictionaryLink", void 0);
__decorate([
    Input(),
    __metadata("design:type", Word)
], RelationsComponent.prototype, "sourceWord", void 0);
__decorate([
    Input(),
    __metadata("design:type", String),
    __metadata("design:paramtypes", [String])
], RelationsComponent.prototype, "relationsLink", null);
RelationsComponent = __decorate([
    Component({
        selector: 'word-relations',
        templateUrl: './Relations.html'
    }),
    __metadata("design:paramtypes", [ActivatedRoute,
        Router,
        AlertService,
        TranslateService,
        DictionaryService])
], RelationsComponent);
export { RelationsComponent };
//# sourceMappingURL=relations.component.js.map