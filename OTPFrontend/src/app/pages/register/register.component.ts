import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})

export class RegisterComponent implements OnInit {
  form! : FormGroup;

  constructor (
    private formBuilder: FormBuilder,
    private router: Router,
    private authService: AuthService,
    private toastr: ToastrService
  ) {

  }

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      first_name: '',
      last_name: '',
      email: '',
      password: '',
      password_confirm: '',
    });
  }

  submit() {
    this.authService.register(this.form.getRawValue()).subscribe({
      next: () => {
        this.router.navigate(['/login'])
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
