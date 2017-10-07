var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Directive, HostListener, ElementRef } from '@angular/core';
var UIToggleDirective = (function () {
    function UIToggleDirective(el) {
        this.el = el;
    }
    UIToggleDirective.prototype.onClick = function () {
        var el = $(this.el.nativeElement);
        var target = $(el.data('target'));
        var targetClass = el.data('class');
        target.toggleClass(targetClass);
        var html = $('html');
        if (html.hasClass('no-focus')) {
            el.blur();
        }
    };
    return UIToggleDirective;
}());
__decorate([
    HostListener('click'),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", []),
    __metadata("design:returntype", void 0)
], UIToggleDirective.prototype, "onClick", null);
UIToggleDirective = __decorate([
    Directive({ selector: '[ui-toggle]' }),
    __metadata("design:paramtypes", [ElementRef])
], UIToggleDirective);
export { UIToggleDirective };
var SideBarToggleDirective = (function () {
    function SideBarToggleDirective(el) {
        this.el = el;
    }
    SideBarToggleDirective.prototype.onClick = function () {
        var windowWidth = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
        var isWideWindow = windowWidth > 991;
        var el = $(this.el.nativeElement);
        var page = $('#page-container');
        var action = el.data('action');
        if (action == "sidebar_mini_toggle") {
            if (isWideWindow) {
                page.toggleClass('sidebar-mini');
            }
        }
        if (action == "sidebar_close") {
            if (isWideWindow) {
                page.removeClass('sidebar-o');
            }
            else {
                page.removeClass('sidebar-o-xs');
            }
        }
        else {
            if (isWideWindow) {
                page.toggleClass('sidebar-o');
            }
            else {
                page.toggleClass('sidebar-o-xs');
            }
        }
    };
    return SideBarToggleDirective;
}());
__decorate([
    HostListener('click'),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", []),
    __metadata("design:returntype", void 0)
], SideBarToggleDirective.prototype, "onClick", null);
SideBarToggleDirective = __decorate([
    Directive({ selector: '[sidebar-toggle]' }),
    __metadata("design:paramtypes", [ElementRef])
], SideBarToggleDirective);
export { SideBarToggleDirective };
//# sourceMappingURL=ui-toggle.directive.js.map