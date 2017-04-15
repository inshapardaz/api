import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ThesaurusComponent } from './thesaurus.component';

@NgModule({
  imports: [CommonModule],
  declarations: [
    ThesaurusComponent
  ],
  providers: [],
  exports: [ThesaurusComponent]
})
export class ThesaurusModule {}