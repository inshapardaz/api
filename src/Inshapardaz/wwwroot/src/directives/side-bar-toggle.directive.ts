import { Directive, HostListener, ElementRef } from '@angular/core';

@Directive({ selector: '[sidebar-toggle]' })
export class SideBarToggleDirective {
    el : ElementRef;
    
    constructor(el: ElementRef) { 
        this.el = el;
    }

    @HostListener('click') onClick() {
        var windowWidth = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
        var isWideWindow = windowWidth > 991;

        var el = $(this.el.nativeElement);        
        var page  = $('#page-container');
        var action = el.data('action');
        if (action == "sidebar_mini_toggle"){
            if (isWideWindow) {
                page.toggleClass('sidebar-mini');
            }
        } if (action == "sidebar_close") {
            if (isWideWindow) {
                page.removeClass('sidebar-o');
            } else {
                page.removeClass('sidebar-o-xs');
            }
        } else {
            if (isWideWindow) {
                page.toggleClass('sidebar-o');
            } else {
                page.toggleClass('sidebar-o-xs');
            }
        }
    }
}