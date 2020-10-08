import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedComponent } from './shared.component';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';

@NgModule({
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(),
    ToastrModule.forRoot({ 
      positionClass: 'toast-bottom-right'
     })
  ],
  declarations: [
    SharedComponent
  ],
  exports: [
    BsDropdownModule,
    ToastrModule
  ]
})
export class SharedModule { }
