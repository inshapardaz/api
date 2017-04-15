import {Component, Input} from '@angular/core'

@Component({
    selector : 'footer',
    templateUrl : 'app/components/footer/footer.html'
})

export class FooterComponent{
    @Input() miniFooter:boolean = true;
    constructor(){
    }
}