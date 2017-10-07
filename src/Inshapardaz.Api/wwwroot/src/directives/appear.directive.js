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
import "../lib/jquery.appear.min";
var AppearDirective = (function () {
    function AppearDirective(el) {
        this.el = el;
    }
    AppearDirective.prototype.ngAfterViewInit = function () {
        var html = $('html');
        var windowW = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
        var thisEl = $(this.el.nativeElement);
        var targetClass = thisEl.data('class') ? thisEl.data('class') : 'animated fadeIn';
        var offset = thisEl.data('offset') ? thisEl.data('offset') : 0;
        var timeout = (html.hasClass('ie9') || windowW < 992) ? 0 : (thisEl.data('timeout') ? thisEl.data('timeout') : 0);
        thisEl.appear(function () {
            setTimeout(function () {
                thisEl
                    .removeClass('visibility-hidden')
                    .addClass(targetClass);
            }, timeout);
        }, { accY: offset });
    };
    return AppearDirective;
}());
AppearDirective = __decorate([
    Directive({
        selector: '[appear]'
    }),
    __metadata("design:paramtypes", [ElementRef])
], AppearDirective);
export { AppearDirective };
//# sourceMappingURL=appear.directive.js.map