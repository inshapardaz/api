var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Injectable } from '@angular/core';
import { Router, NavigationStart } from '@angular/router';
import { Subject } from 'rxjs/Subject';
import * as $ from "jquery";
var AlertService = (function () {
    function AlertService(router) {
        var _this = this;
        this.router = router;
        this.subject = new Subject();
        this.keepAfterRouteChange = false;
        router.events.subscribe(function (event) {
            if (event instanceof NavigationStart) {
                if (_this.keepAfterRouteChange) {
                    _this.keepAfterRouteChange = false;
                }
                else {
                    _this.clear();
                }
            }
        });
    }
    AlertService.prototype.showNotification = function (notifyIcon, notifyMessage, notifyType, notifyFrom, notifyAlign) {
        $['notify']({
            icon: notifyIcon,
            message: notifyMessage,
        }, {
            element: 'body',
            type: notifyType,
            allow_dismiss: true,
            newest_on_top: true,
            showProgressbar: false,
            placement: {
                from: notifyFrom,
                align: notifyAlign
            },
            offset: 20,
            spacing: 10,
            z_index: 1033,
            delay: 5000,
            timer: 1000,
            animate: {
                enter: 'animated fadeIn',
                exit: 'animated fadeOutDown'
            }
        });
    };
    AlertService.prototype.success = function (message) {
        console.log('show success');
        var icon = 'fa fa-check';
        this.showNotification(icon, message, 'success', 'bottom', 'center');
    };
    AlertService.prototype.error = function (message) {
        console.log('show error');
        var icon = 'fa fa-times';
        this.showNotification(icon, message, 'danger', 'bottom', 'center');
    };
    AlertService.prototype.info = function (message, keepAfterRouteChange) {
        if (keepAfterRouteChange === void 0) { keepAfterRouteChange = false; }
        console.log('show info');
        var icon = 'fa fa-info-circle';
        this.showNotification(icon, message, 'info', 'bottom', 'center');
    };
    AlertService.prototype.warn = function (message, keepAfterRouteChange) {
        if (keepAfterRouteChange === void 0) { keepAfterRouteChange = false; }
        var icon = 'fa fa-warning';
        this.showNotification(icon, message, 'warning', 'bottom', 'center');
    };
    AlertService.prototype.clear = function () {
        // clear alerts
        this.subject.next();
    };
    return AlertService;
}());
AlertService = __decorate([
    Injectable(),
    __metadata("design:paramtypes", [Router])
], AlertService);
export { AlertService };
//# sourceMappingURL=alert.service.js.map