import { Directive, HostListener, ElementRef } from '@angular/core';

@Directive({ selector: '[rtl]' })
export class RightToLeftDirective {
    el : ElementRef;
    
    constructor(el: ElementRef) { 
        this.el = el;
    }
}