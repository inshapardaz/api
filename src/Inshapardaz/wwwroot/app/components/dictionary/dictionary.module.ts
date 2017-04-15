import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';

import { DictionaryService } from '../../services/dictionary.service';

import { DictionariesComponent } from './dictionaries.component';
import { DictionaryDetailComponent } from './dictionaryDetail.Compontent';
import { DictionaryComponent } from './dictionary.component';
import { DictionarySidebarComponent } from './sidebar.component';
import { DictionaryOverviewComponent } from './overview.component';
import { WordListComponent } from './wordList.component';
import { WordComponent } from './word.component';


@NgModule({
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  declarations: [
    WordListComponent,
    DictionariesComponent,
    DictionaryDetailComponent,
    DictionaryComponent,
    DictionarySidebarComponent,
    DictionaryOverviewComponent,
    WordComponent,
  ],
  providers: [
    DictionaryService
  ],
  exports: [DictionaryComponent]
})
export class DictionaryModule { }