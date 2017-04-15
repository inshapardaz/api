import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TranslatorComponent } from './translator.component';

@NgModule({
  imports: [CommonModule],
  declarations: [
    TranslatorComponent
  ],
  providers: [],
  exports: [TranslatorComponent]
})
export class TranslatorModule {}