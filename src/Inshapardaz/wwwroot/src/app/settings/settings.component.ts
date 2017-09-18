import { Component, Input } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import {TranslateService} from '@ngx-translate/core';

// import amethyst from './../../assets/css/themes/amethyst.min.css';
// import city from '../../assets/css/themes/city.min.css';
// import flat from '../../assets/css/themes/flat.min.css';
// import modern from '../../assets/css/themes/modern.min.css';
// import smooth from '../../assets/css/themes/smooth.min.css'  ;

@Component({
    selector: 'settings',
    templateUrl: './settings.html'})

export class SettingsComponent {
    public selectedTheme : string = "";
    constructor(
        public translate: TranslateService
    ) {
    }

    setLanguage(lang) : void{
        this.translate.use(lang);
        localStorage.setItem('ui-lang', lang);
    }

    setTheme(theme) : void{
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
    }
}

export class ThemeSetting {
    constructor(public title : string, public css : string, colorClass : string){
    }
}
