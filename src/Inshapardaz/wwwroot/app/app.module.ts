import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { LocationStrategy, HashLocationStrategy } from "@angular/common";
import { HttpModule, JsonpModule } from '@angular/http';
import { FormsModule } from '@angular/forms';
import { AUTH_PROVIDERS }      from 'angular2-jwt';

import { AppComponent} from './components/app/app.component';
import { routing  } from './app.routes';

import { DropDownDirective, RowExpandCollapseDirective, HoverDropDownMenuDirective, SidebarToggleDirective, ThemeDropDownItemDirective} from './components/directives';

import { Auth } from './services/auth.service';

import {HeaderComponent} from './components/header/header.component';
import {FooterComponent} from './components/footer/footer.component';
import {AppsModalComponent} from './components/appsModal/appsModal.component';

import { HomeComponent } from './components/home/home.component';
import { SearchComponent } from './components/search/search.component';
import { PageNotFoundComponent } from "./components/pages/notfound.component";

import { DictionaryModule } from './components/dictionary/dictionary.module';
import { ThesaurusModule } from './components/thesaurus/thesaurus.module';
import { TranslatorModule } from './components/translator/translator.module'; 
import { EditorModule } from './components/editor/editor.module';

@NgModule({
  imports: [
    BrowserModule,
    routing,
    FormsModule,
    HttpModule,
    JsonpModule,
    DictionaryModule,
    ThesaurusModule,
    TranslatorModule,
    EditorModule
  ],
  providers: [
    HttpModule,
    AUTH_PROVIDERS,
    Auth
  ],
  declarations: [
    HeaderComponent,
    FooterComponent,
    AppsModalComponent,

    DropDownDirective,
    HoverDropDownMenuDirective,
    ThemeDropDownItemDirective,
    SidebarToggleDirective,
    RowExpandCollapseDirective,

    AppComponent,
    HomeComponent,
    SearchComponent,
    PageNotFoundComponent
  ],

  bootstrap: [AppComponent]
})
export class AppModule { }   