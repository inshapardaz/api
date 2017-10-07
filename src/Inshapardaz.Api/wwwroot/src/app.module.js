var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { NgModule } from '@angular/core';
import { BrowserModule, Title } from '@angular/platform-browser';
import { HttpModule, JsonpModule } from '@angular/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HttpClient } from '@angular/common/http';
// 3rd party references
import { TranslateModule, TranslateLoader } from "@ngx-translate/core";
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { SweetAlert2Module } from '@toverux/ngsweetalert2';
import { Ng2AutoCompleteModule } from 'ng2-auto-complete';
import { OAuthModule } from 'angular-oauth2-oidc';
// Services
import { AuthService } from './services/auth.service';
import { DictionaryService } from './services/dictionary.service';
import { AlertService } from './services/alert.service';
// Directives
import { UIToggleDirective } from './directives/ui-toggle.directive';
import { SideBarToggleDirective } from './directives/side-bar-toggle.directive';
import { AppearDirective } from './directives/appear.directive';
import { RightToLeftDirective } from './directives/right-to-left.directive';
// Components
import { AppComponent } from './app/app.component';
import { CallbackComponent } from './app/callback/callback.component';
import { ProfileComponent } from './app/profile/profile.component';
import { UnauthorisedComponent } from './app/error/unauthorised/unauthorised.component';
import { ServerErrorComponent } from './app/error/serverError/serverError.component';
import { HomeComponent } from './app/home/home.component';
import { HeaderComponent } from './app/header/header.component';
import { SidebarComponent } from './app/sidebar/sidebar.component';
import { FooterComponent } from './app/footer/footer.component';
import { SettingsComponent } from './app/settings/settings.component';
import { DictionariesComponent } from './app/dictionary/dictionaries/dictionaries.component';
import { DictionaryComponent, DictionaryByLinkComponent } from './app/dictionary/dictionary/dictionary.component';
import { EditDictionaryComponent } from './app/dictionary/edit-dictionary/edit-dictionary.component';
import { EditWordComponent } from './app/dictionary/edit-word/edit-word.component';
import { WordComponent } from './app/dictionary/word/word.component';
import { WordDetailsComponent } from './app/dictionary/wordDetail/wordDetail.component';
import { EditWordDetailComponent } from './app/dictionary/edit-wordDetail/edit-wordDetail.component';
import { RelationsComponent } from './app/dictionary/relations/relations.component';
import { EditWordRelationComponent } from './app/dictionary/edit-wordRelation/edit-wordRelation.component';
import { MeaningsComponent } from './app/dictionary/meanings/meanings.component';
import { EditMeaningComponent } from './app/dictionary/edit-meaning/edit-meaning.component';
import { WordTranslationsComponent } from './app/dictionary/translations/translations.component';
import { EditWordTranslationComponent } from './app/dictionary/edit-translation/edit-translation.component';
import { ThesaurusComponent } from './app/thesaurus/thesaurus.component';
import { TranslationsComponent } from './app/translations/translations.component';
import { routing } from './app.routes';
export function HttpLoaderFactory(http) {
    return new TranslateHttpLoader(http);
}
var AppModule = (function () {
    function AppModule() {
    }
    return AppModule;
}());
AppModule = __decorate([
    NgModule({
        imports: [
            BrowserModule,
            routing,
            FormsModule,
            ReactiveFormsModule,
            HttpModule,
            HttpClientModule,
            JsonpModule,
            TranslateModule.forRoot({
                loader: {
                    provide: TranslateLoader,
                    useFactory: HttpLoaderFactory,
                    deps: [HttpClient]
                }
            }),
            OAuthModule.forRoot(),
            SweetAlert2Module,
            Ng2AutoCompleteModule
        ],
        providers: [
            HttpModule,
            Title,
            AuthService,
            DictionaryService,
            AlertService
        ],
        declarations: [
            UIToggleDirective,
            SideBarToggleDirective,
            RightToLeftDirective,
            AppearDirective,
            AppComponent,
            CallbackComponent,
            ProfileComponent,
            UnauthorisedComponent,
            ServerErrorComponent,
            HomeComponent,
            HeaderComponent,
            SidebarComponent,
            FooterComponent,
            SettingsComponent,
            DictionariesComponent,
            DictionaryByLinkComponent,
            DictionaryComponent,
            EditDictionaryComponent,
            EditWordComponent,
            EditWordDetailComponent,
            EditWordRelationComponent,
            WordComponent,
            WordDetailsComponent,
            RelationsComponent,
            MeaningsComponent,
            EditMeaningComponent,
            WordTranslationsComponent,
            EditWordTranslationComponent,
            ThesaurusComponent,
            TranslationsComponent
        ],
        bootstrap: [AppComponent]
    })
], AppModule);
export { AppModule };
//# sourceMappingURL=app.module.js.map