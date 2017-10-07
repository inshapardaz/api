var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from '../../services/auth.service';
var HeaderComponent = (function () {
    function HeaderComponent(router, auth, translate) {
        this.router = router;
        this.auth = auth;
        this.translate = translate;
        this.miniHeader = false;
        this.searchText = "";
    }
    HeaderComponent.prototype.ngOnInit = function () {
    };
    HeaderComponent.prototype.onSearch = function (event) {
        if (event.keyCode == 13) {
            this.router.navigate(['/search', this.searchText]);
        }
    };
    return HeaderComponent;
}());
__decorate([
    Input(),
    __metadata("design:type", Boolean)
], HeaderComponent.prototype, "miniHeader", void 0);
HeaderComponent = __decorate([
    Component({
        selector: 'header',
        templateUrl: './header.component.html'
    }),
    __metadata("design:paramtypes", [Router,
        AuthService,
        TranslateService])
], HeaderComponent);
export { HeaderComponent };
//# sourceMappingURL=header.component.js.map