import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup;
  maxDate: Date;
  validationErrors: string[] = [];

  constructor(private accountService: AccountService, 
              private fb: FormBuilder,
              private router: Router) { }

  ngOnInit() {
    this.initialiseForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18); // sets date picker date no earlier than 18 years ago
  }

  initialiseForm() {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      gender: ['male'],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: [
                  '', 
                  [
                    Validators.required, 
                    Validators.minLength(4), 
                    Validators.maxLength(8)
                  ] 
                ],
      confirmPassword: [
                        '', 
                        [
                          Validators.required, 
                          this.matchValues('password') // 'this.matchValues()' returns type control : AbstractControl
                        ] 
                      ] 
      
    })
  }

  // the [matchTo] will be the 'password' control
  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      // [control?.value] will be the 'confirmPassword' control
      return control?.value === control?.parent?.controls[matchTo].value 
      ? null : {isMatching: true} // 'isMatching' is abortuary name to say the matching value
    }
  }

  register() {
    this.accountService.register(this.registerForm.value).subscribe(response => {
      this.router.navigateByUrl('/members');
    }, error => {
      this.validationErrors = error;
    });
  }

  cancel() {
    // turn off register mode in homeComponent
    this.cancelRegister.emit(false);
  }

}
