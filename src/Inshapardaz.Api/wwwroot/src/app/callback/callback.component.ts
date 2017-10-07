import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';

@Component({
    selector: 'callback',
    templateUrl: './callback.html'
})

export class CallbackComponent {
     constructor(private router: Router,){
        this.router.navigateByUrl('/home');
     }
}
