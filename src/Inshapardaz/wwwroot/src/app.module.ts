import { NgModule } from '@angular/core';
import { BrowserModule, Title } from '@angular/platform-browser';
import { LocationStrategy, HashLocationStrategy } from "@angular/common";
import { HttpModule, JsonpModule } from '@angular/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { TranslateModule } from "ng2-translate/ng2-translate";

import { AuthService } from './services/auth.service';
import { DictionaryService } from './services/dictionary.service';
import { AlertService } from './services/alert.service';

import { UIToggleDirective } from './directives/ui-toggle.directive';
import { SideBarToggleDirective } from './directives/side-bar-toggle.directive';
import { AppearDirective } from './directives/appear.directive';
import { RightToLeftDirective } from './directives/right-to-left.directive';

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
import { ThesaurusComponent } from './app/thesaurus/thesaurus.component';
import { TranslationsComponent } from './app/translations/translations.component';

import { SweetAlert2Module } from '@toverux/ngsweetalert2';

import { routing } from './app.routes';

@NgModule({
  imports: [
    BrowserModule,
    routing,
    FormsModule,
    ReactiveFormsModule,
    HttpModule,
    JsonpModule,
    TranslateModule.forRoot(),
    SweetAlert2Module
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
    ThesaurusComponent,
    TranslationsComponent
  ],

  bootstrap: [AppComponent]
})

export class AppModule { }