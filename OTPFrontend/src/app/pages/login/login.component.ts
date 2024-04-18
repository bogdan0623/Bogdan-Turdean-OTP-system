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
    // img: ''
  }

  ngOnInit(): void {

  }

  onLogin(data: any) {
    this.loginData = data;

    // if(data.oupauth_url) {
    //   qrcode.toDataURL(data.oupauth_url, (err: any, img: string) => {
    //     this.loginData.img = img;
    //   })
    // }
  }
  
}
