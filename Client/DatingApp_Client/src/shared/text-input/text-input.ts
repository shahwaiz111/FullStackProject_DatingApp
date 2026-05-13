import { Component, input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  imports: [ReactiveFormsModule],
  templateUrl: './text-input.html',
  styleUrl: './text-input.css',
})
// 'ControlValueAccessor' This is going to tell Angular that this component is a form control..
export class TextInput implements ControlValueAccessor {
  
  // we're going to take in a couple of input properties into this..
  label = input<string>('');
  type = input<string>('text');
  maxDate = input<string>('');

  constructor(@Self() public ngControl: NgControl){
    this.ngControl.valueAccessor = this;
  }

  writeValue(obj: any): void {
  }
  registerOnChange(fn: any): void {
  }
  registerOnTouched(fn: any): void {
  }

  get Control(): FormControl{
    return this.ngControl.control as FormControl;
  }

}
