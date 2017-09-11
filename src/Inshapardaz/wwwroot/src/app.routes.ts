import { ModuleWithProviders }  from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent }      from './app/home/home.component';
import { DictionariesComponent }      from './app/dictionary/dictionaries/dictionaries.component';
import { DictionaryComponent,DictionaryByLinkComponent }      from './app/dictionary/dictionary/dictionary.component';
import { WordComponent }      from './app/dictionary/word/word.component';
import { SettingsComponent } from './app/settings/settings.component';

import { CallbackComponent }      from './app/callback/callback.component';
import { ProfileComponent } from './app/profile/profile.component';

import { ThesaurusComponent }      from './app/thesaurus/thesaurus.component';
import { TranslationsComponent }      from './app/translations/translations.component';
import { UnauthorisedComponent } from './app/error/unauthorised/unauthorised.component';
import { ServerErrorComponent } from './app/error/serverError/serverError.component';

const appRoutes: Routes = [
    { path: '',            component: HomeComponent },
    { path: 'home',        component: HomeComponent },
    { path: 'callback',    component: CallbackComponent },
    { path: 'profile',      component: ProfileComponent },
    { path: 'settings',     component: SettingsComponent },
    
    { path: 'dictionaries',        component: DictionariesComponent},
    { path: 'dictionaryLink/:link',      component: DictionaryByLinkComponent},
    { path: 'dictionary/:id',      component: DictionaryComponent},
    { path: 'dictionary/:id/:page',      component: DictionaryComponent},
    { path: 'word/:id', component: WordComponent },
    { path: 'thesaurus',     component: ThesaurusComponent },
    { path: 'translations',    component: TranslationsComponent },
    { path: 'error/unauthorised', component : UnauthorisedComponent },
    { path: 'error/servererror', component : ServerErrorComponent }
];

export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);