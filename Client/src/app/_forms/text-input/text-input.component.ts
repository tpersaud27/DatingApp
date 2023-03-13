import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css'],
})
export class TextInputComponent implements ControlValueAccessor {
  @Input() label = ' ';
  @Input() type = 'text';

  // Self allows us to ensure each ngControl is unique
  constructor(@Self() public ngControl: NgControl) {
    // This represents out text input component
    // We will pass this one of our controls, set the value accessor to the textInputComponent
    // which implements the ControlValueAccessor
    // which then allows us to writeValues, registerOnChange, and registerOnTouched
    this.ngControl.valueAccessor = this;
  }

  writeValue(obj: any): void {}

  registerOnChange(fn: any): void {}

  registerOnTouched(fn: any): void {}

  get control(): FormControl {
    return this.ngControl.control as FormControl;
  }
}
