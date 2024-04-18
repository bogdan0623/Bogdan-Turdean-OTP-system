import { Component, EventEmitter, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-form',
  templateUrl: './form.component.html',
  styleUrl: './form.component.css'
})
export class FormComponent {

  @Output('onLogin') onLogin = new EventEmitter();

  form! : FormGroup;

  constructor (
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private toastr: ToastrService
  ) {

  }

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      email: '',
      password: '',
    });
  }

  submit() {
    this.authService.login(this.form.getRawValue()).subscribe({
      next: res => {
        this.onLogin.emit(res)
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
