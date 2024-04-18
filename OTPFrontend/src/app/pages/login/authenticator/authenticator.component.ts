import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-authenticator',
  templateUrl: './authenticator.component.html',
  styleUrl: './authenticator.component.css'
})
export class AuthenticatorComponent implements OnInit{

  @Input('loginData') loginData = {
    id: 0,
  };

  form! : FormGroup;
  otpForm! : FormGroup;

  constructor (
    private formBuilder: FormBuilder,
    private router: Router,
    private authService: AuthService,
    private toastr: ToastrService
  ) {

  }

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      code: '',
    });

    this.otpForm = this.formBuilder.group({
      expires_in: '',
    });
  }

  submit() {
    const formData = this.form.getRawValue();
    const data = this.loginData;

    this.authService.authenticatorLogin({
      ...data,
      ...formData
    }).subscribe({
      next: (res: any) => {
        this.authService.accessToken = res.token;
        AuthService.behaviorSubject.next(true);
        this.router.navigate(['/']);
      },
      error: err => {
        this.toastr.error(err.error.message, 'ERROR', {
          timeOut: 3000,
          closeButton: true,
          tapToDismiss: false
        });
      }
    });
  }

  showMessage() {
    const data = this.loginData;
    const otpFormData = this.otpForm.getRawValue();
    this.authService.generateOtp({
      ...data,
      ...otpFormData
    }).subscribe({
      next: (res: any) => {
        this.toastr.info(res.code, 'Your OTP', {
          timeOut: otpFormData.expires_in * 1000,
          closeButton: true,
          tapToDismiss: false
        });
      },
      error: err => {
        this.toastr.error(err.error.message, 'ERROR', {
          timeOut: 3000,
          closeButton: true,
          tapToDismiss: false
        });
      }
    });
  }
}
