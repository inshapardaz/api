var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Directive, ElementRef } from '@angular/core';
var RightToLeftDirective = (function () {
    function RightToLeftDirective(el) {
        this.el = el;
    }
    return RightToLeftDirective;
}());
RightToLeftDirective = __decorate([
    Directive({ selector: '[rtl]' }),
    __metadata("design:paramtypes", [ElementRef])
], RightToLeftDirective);
export { RightToLeftDirective };
//# sourceMappingURL=right-to-left.directive.js.map