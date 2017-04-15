import { Component, Input } from '@angular/core';

@Component({
    selector : 'app-modal',
    templateUrl : 'app/components/appsModal/appsModal.html'
})

export class AppsModalComponent{
    @Input() currentRoute: string = "";
    constructor(){
    }
}