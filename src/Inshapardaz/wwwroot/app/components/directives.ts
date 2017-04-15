import {Directive, ElementRef, Input, Renderer, HostListener} from '@angular/core'

import {CookieService} from 'angular2-cookie/core';

@Directive({
    selector: '[dropdown]'
})

export class DropDownDirective {
    constructor(el: ElementRef, renderer: Renderer) {
    }

    @HostListener('click', ['$event.target'])
    onClick(btn) {
        var $el = jQuery(btn);
        if ($el.data('target') == null) {
            $el = $el.parent();
        }

        if ($el.data('target') != null) {
            $($el.data('target').toString()).toggleClass($el.data('class').toString());

            var $lHtml = jQuery('html');
            // Remove focus from submenu link
            if ($lHtml.hasClass('no-focus')) {
                $el.blur();
            }
        }
    }
}

@Directive({
    selector: '[sidebar-toggle]'
})

export class SidebarToggleDirective {
    constructor(el: ElementRef, renderer: Renderer) {
        console.log('element Binding element ');
    }

    @HostListener('click', ['$event.target'])
    onClick(btn) {
        var $el = jQuery(btn);
        var $lPage = jQuery('#page-container');
        var $windowW = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;

        if ($windowW > 991) {
            $lPage.toggleClass('sidebar-o');
        } else {
            $lPage.toggleClass('sidebar-o-xs');
        }
    }
}


@Directive({
    selector: '[hover-dropdown-menu]'
})

export class HoverDropDownMenuDirective {
    constructor(el: ElementRef, renderer: Renderer) {
    }

    @HostListener('click', ['$event.target'])
    onClick(btn) {
        // Get link
        var $link = jQuery(btn);

        // Get link's parent
        var $parentLi = $link.parent('li');

        if ($parentLi.hasClass('open')) { // If submenu is open, close it..
            $parentLi.removeClass('open');
        } else { // .. else if submenu is closed, close all other (same level) submenus first before open it
            $link
                .closest('ul')
                .find('> li')
                .removeClass('open');

            $parentLi
                .addClass('open');
        }

        var $lHtml = jQuery('html');

        // Remove focus from submenu link
        if ($lHtml.hasClass('no-focus')) {
            $link.blur();
        }

        return false;
    }
}

@Directive({
    selector: '[theme-dropdown-item]',
    providers: [CookieService],
})

export class ThemeDropDownItemDirective {
    constructor(el: ElementRef, renderer: Renderer,
        private _cookieService: CookieService) {

        var $this = jQuery(el.nativeElement);
        var $lPage = jQuery('#page-container');
        var $mytheme = $this.data('theme');
        var $cssTheme = jQuery('#css-theme');
        var $cookies = $lPage.hasClass('enable-cookies') ? true : false;

        // If cookies are enabled
        if ($cookies) {
            var $theme = _cookieService.get('colorTheme') ? _cookieService.get('colorTheme') : null;

            // Update color theme
            if ($theme) {
                if ($theme === 'default') {
                    if ($cssTheme.length) {
                        $cssTheme.remove();
                    }
                } else {
                    if ($cssTheme.length) {
                        $cssTheme.attr('href', $theme);
                    } else {
                        jQuery('#css-main')
                            .after('<link rel="stylesheet" id="css-theme" href="' + $theme + '">');
                    }
                }
            }

            $cssTheme = jQuery('#css-theme');
        }

        if ($cssTheme.attr('href') == $mytheme)
            $this.parent('li')
                .addClass('active');
    }

    @HostListener('click', ['$event.target'])
    onClick(btn) {

        var $this = jQuery(btn);
        var $lPage = jQuery('#page-container');
        var $theme = $this.data('theme');
        var $cssTheme = jQuery('#css-theme');
        var $cookies = $lPage.hasClass('enable-cookies') ? true : false;

        // Set this color theme link as active
        jQuery('[data-toggle="theme"]')
            .parent('li')
            .removeClass('active');

        jQuery('[data-toggle="theme"][data-theme="' + $theme + '"]')
            .parent('li')
            .addClass('active');

        // Update color theme
        if ($theme === 'default') {
            if ($cssTheme.length) {
                $cssTheme.remove();
            }
        } else {
            if ($cssTheme.length) {
                $cssTheme.attr('href', $theme);
            } else {
                jQuery('#css-main')
                    .after('<link rel="stylesheet" id="css-theme" href="' + $theme + '">');
            }
        }

        $cssTheme = jQuery('#css-theme');

        // If cookies are enabled, save the new active color theme
        if ($cookies) {
            this._cookieService.put('colorTheme', $theme);
        }

        return false;
    }
}

@Directive({
    selector: '[row-expand-collapse]'
})

export class RowExpandCollapseDirective {
    $table: any;
    constructor(el: ElementRef, renderer: Renderer) {
        this.$table = jQuery(el.nativeElement);
    }

    @HostListener('click', ['$event.target'])
    onClick(btn) {
        var $row = jQuery(btn);
        var $tbody = $row.closest('tbody');

        if (!$tbody.hasClass('open')) {
            jQuery('tbody', this.$table).removeClass('open');
        }

        $tbody.toggleClass('open');
    }
}