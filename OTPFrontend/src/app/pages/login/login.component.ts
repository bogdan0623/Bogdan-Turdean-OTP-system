import { Component, OnInit } from '@angular/core';
import * as qrcode from 'qrcode';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})

export class LoginComponent implements OnInit {

  loginData = {
    id: 0,
  }

  ngOnInit(): void {

  }

  onLogin(data: any) {
    this.loginData = data;
  }
  
}
