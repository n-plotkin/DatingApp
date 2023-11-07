import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})
export class TextInputComponent implements ControlValueAccessor{

  @Input() label = '';
  @Input() type = 'text';


  //We want to make sure our ngControl is unique to the DOM
  //therefore we use the @Self decorator
  constructor(@Self() public ngControl: NgControl) {
    //we pass it our ngControl, and then we assign the ngControl's value accessor 
    //to this very same class
    this.ngControl.valueAccessor = this;
  }

  writeValue(obj: any): void {
  }

  registerOnChange(fn: any): void {
  }

  registerOnTouched(fn: any): void {
  }


  //casting our control into a form control
  //only necessary b/c typescript strict isn't sure if it's really a form control otherwise.
  get control(): FormControl {
    return this.ngControl.control as FormControl;
  }

}
