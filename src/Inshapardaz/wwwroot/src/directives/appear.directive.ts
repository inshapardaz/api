import { Directive, ElementRef, Input } from '@angular/core';
import "../lib/jQuery.appear.min";
@Directive({ 
    selector: '[appear]' }
)
export class AppearDirective {
    el: ElementRef;

    constructor(el: ElementRef) {
        this.el = el;
    }

    ngAfterViewInit() {
        var html = $('html');
        var windowW = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
        var thisEl  : any = $(this.el.nativeElement);
        var targetClass = thisEl.data('class') ? thisEl.data('class') : 'animated fadeIn';
        var offset = thisEl.data('offset') ? thisEl.data('offset') : 0;
        var timeout = (html.hasClass('ie9') || windowW < 992) ? 0 : (thisEl.data('timeout') ? thisEl.data('timeout') : 0);

        thisEl.appear(() => {
            setTimeout(() => {

                thisEl
                    .removeClass('visibility-hidden')
                    .addClass(targetClass);
            }, timeout);
        }, { accY: offset });
    }
}