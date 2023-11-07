import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})

export class RegisterComponent {
  //this is passed to us in the home component html template, which has access to it.
  @Input() usersFromHomeComponent: any;

  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup = new FormGroup({});
  maxDate: Date = new Date();
  minDate: Date = new Date();
  validationErrors: string[] | undefined;

  constructor(private accountService: AccountService, private toastr: ToastrService,
    private fb: FormBuilder, private router: Router){

  }

  ngOnInit(){
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear()-18)
    this.minDate.setFullYear(this.minDate.getFullYear()-120)
  }
  
  //reactive forms
  initializeForm(){
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(12)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]],
    });
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => {
        this.registerForm.controls['confirmPassword'].updateValueAndValidity()
      }
    });
  }

  matchValues(matchTo: string): ValidatorFn {
    //We have to return a function, (a validator is a function).
    //form control for reactive forms is derived from AbstractControl, we pass it to our anonymous func
    return (control: AbstractControl) => {
      //control is the thing calling the func, for us "confirmPassword" field.
      //Parent is the whole form
      return control.value === control.parent?.get(matchTo)?.value ? null : {notMatching: true}
    }
  }

  register(){ 
    //we want choppped dob
    const dob = this.getDateOnly(this.registerForm.controls['dateOfBirth'].value);
    // "..." spread from value to values
    const values = {...this.registerForm.value, dateOfBirth: dob};
    
    this.accountService.register(values).subscribe({
      //the request only actually happens when we subscribe to it
      next: () => {
        this.router.navigateByUrl('/members')
      },
      error: error => {
        this.validationErrors = error
      }
    })
  }

  cancel(){
    //this will be outputted to home component html
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob: string | undefined) {
    if (!dob) return;
    let _dob = new Date(dob);
    return new Date(_dob.setMinutes(_dob.getMinutes()-_dob.getTimezoneOffset()))
      .toISOString().slice(0,10);
  }
}
