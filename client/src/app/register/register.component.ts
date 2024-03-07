import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup = new FormGroup({});
  maxDate: Date = new Date();
  validationErrors: string[] | undefined;

  constructor(private accountService: AccountService,
    private toastr: ToastrService, private router: Router,
    public datepipe: DatePipe) {

  }
  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }
  initializeForm() {
    this.registerForm = new FormGroup({
      userName: new FormControl('hello', Validators.required),
      gender: new FormControl('male'),
      city: new FormControl('', Validators.required),
      country: new FormControl('', Validators.required),
      knownAs: new FormControl('', Validators.required),
      dateOfBirth: new FormControl('', Validators.required),
      password: new FormControl('', [Validators.required, Validators.minLength(4),
      Validators.maxLength(8)]),
      cofirmPassword: new FormControl('', [Validators.required, this.matchValues('password')]),
    },
      // {
      //   validators: this.matchValues
      // }
    );
  }
  // matchValues(control: AbstractControl) {
  //   console.log("hhhh");
  //   return control.get('password')?.value === control.get('cofirmPassword')?.value ? null : { notMatching: true };
  // }
  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : { notMatching: true };
      //     confirmPasword.Value == confirmPassword.parent (form).get(password)
    }
  }
  register() {
    const dob = this.getDateOnly(this.registerForm.get('dateOfBirth')?.value);
    const values = { ...this.registerForm.value, dateOfBirth: dob };
    console.log(values);

    this.accountService.register(values).subscribe(
      {
        next: () => {
          this.router.navigateByUrl('/members')
        },
        error: (error) => this.validationErrors = error,
      }
    );
  }
  cancel() {
    this.cancelRegister.emit(false);
  }
  private getDateOnly(dob: string | undefined) {
    return this.datepipe.transform(dob, 'yyyy-MM-dd')
  }
}
