var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
// import amethyst from './../../assets/css/themes/amethyst.min.css';
// import city from '../../assets/css/themes/city.min.css';
// import flat from '../../assets/css/themes/flat.min.css';
// import modern from '../../assets/css/themes/modern.min.css';
// import smooth from '../../assets/css/themes/smooth.min.css'  ;
var SettingsComponent = (function () {
    function SettingsComponent(translate) {
        this.translate = translate;
        this.selectedTheme = "";
    }
    SettingsComponent.prototype.setLanguage = function (lang) {
        this.translate.use(lang);
        localStorage.setItem('ui-lang', lang);
    };
    SettingsComponent.prototype.setTheme = function (theme) {
        // var $cssTheme = $('#css-theme');
        // if (theme === '') {
        //         if ($cssTheme.length) {
        //             $cssTheme.remove();
        //         }
        //     } else {
        //         if ($cssTheme.length) {
        //             $cssTheme.attr('href', theme);
        //         } else {
        //             $('#css-main')
        //                 .after('<link rel="stylesheet" id="css-theme" href="' + theme + '">');
        //         }
        //     }
        console.log("Themes not implemented");
    };
    return SettingsComponent;
}());
SettingsComponent = __decorate([
    Component({
        selector: 'settings',
        templateUrl: './settings.html'
    }),
    __metadata("design:paramtypes", [TranslateService])
], SettingsComponent);
export { SettingsComponent };
var ThemeSetting = (function () {
    function ThemeSetting(title, css, colorClass) {
        this.title = title;
        this.css = css;
    }
    return ThemeSetting;
}());
export { ThemeSetting };
//# sourceMappingURL=settings.component.js.map