import { Component, inject, input, output, signal } from '@angular/core';
import {AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { RegisterCreds, User } from '../../../types/user';
import { AccountService } from '../../../core/services/account-service';
import { JsonPipe } from '@angular/common';
import { TextInput } from "../../../shared/text-input/text-input";
import { Router } from '@angular/router';


@Component({
  selector: 'app-register',
  // imports: [FormsModule],
  imports: [ReactiveFormsModule, JsonPipe, TextInput],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  

  private accountService = inject(AccountService);
  private router = inject(Router);
  // parent to child communication (Signal way)
  // public membersFromHome = input.required<User[]>();

  private fb = inject(FormBuilder);
  // // child to parent communication (Signal way)
  cancelRegister = output<boolean>();
  protected creds = {} as RegisterCreds;
  protected credentialsForm:FormGroup;
  protected profileForm: FormGroup;
  protected currentStep = signal(1);
  protected validationErrors = signal<string[]>([]);

  constructor(){
    this.credentialsForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      displayName: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]],
    });

    this.credentialsForm.controls['password'].valueChanges.subscribe(()=>{
      this.credentialsForm.controls['confirmPassword'].updateValueAndValidity();
    });

    // it will allow us to to demonstrate the concept of a stepper form..
    this.profileForm = this.fb.group({
      gender: ['male', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required]
    })
  }

  // ngOnInit(): void {
  //   this.initializeForm();
  // }

  // initializeForm(){
    // }

  matchValues(matchTo:string): ValidatorFn{
    return (control:AbstractControl) : ValidationErrors | null => {
      const parent = control.parent;
      if (!parent) return null;

      const matchValue = parent.get(matchTo)?.value;
      return control.value === matchValue? null : {passwordMisMatch:true}
    }
  }

  nextStep(){
    if (this.credentialsForm.valid){
      this.currentStep.update(prevStep => prevStep + 1);
    }
  }

  prevStep(){
    this.currentStep.update(prevStep => prevStep -1)
  }

  getMaxDate(){
    const today = new Date();
    today.setFullYear(today.getFullYear() - 18);
    return today.toISOString().split('T')[0];
  }
  

  register(){
    // console.log(this.credentialsForm.value); // output of the form in the browser..

    if (this.profileForm.valid && this.credentialsForm.valid){
      const formData = {...this.credentialsForm.value, ...this.profileForm.value};

        this.accountService.register(formData).subscribe({
      next: ()=> {
        this.router.navigateByUrl('/members');
        // this.cancel();
      },
      error: error=> {
        console.log(error);
        this.validationErrors.set(error);
      }
    })
      // console.log('Form data: ', formData);
    }

     
  }

  cancel(){
    // console.log('cancelled!');
    this.cancelRegister.emit(false)
  }
}
