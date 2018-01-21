import { Component } from '@angular/core';
import { AuthService } from './auth.service';
import { UrlManagerService } from './url-manager.service';

@Component({
    selector: 'app-root',
    templateUrl: 'app.component.html',
    styleUrls: ['app.component.css']
})

export class AppComponent {
    constructor(public readonly auth: AuthService, private readonly urlManager : UrlManagerService) {
        auth.handleAuthenticationWithHash();
     }

    isGemini = () =>  this.urlManager.isGemini();
}