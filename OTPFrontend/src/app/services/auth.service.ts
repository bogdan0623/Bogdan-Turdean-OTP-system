import { HttpClient } from '@angular/common/http';
import { EventEmitter, Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  static authEmitter = new EventEmitter<boolean>();
  static behaviorSubject = new BehaviorSubject(false);

  accessToken = '';

  constructor(private http: HttpClient) { }

  register(body: any){
    return this.http.post('http://localhost:8000/api/register', body);
  }

  login(body: any){
    return this.http.post('http://localhost:8000/api/login', body);
  }

  authenticatorLogin(body: any){
    return this.http.post('http://localhost:8000/api/two-factor', body, {withCredentials: true});
  }

  user(){
    return this.http.get('http://localhost:8000/api/user');
  }

  generateOtp(body: any){
    return this.http.post('http://localhost:8000/api/otp', body);
  }

  refresh(){
    return this.http.post('http://localhost:8000/api/refresh', {}, {withCredentials: true});
  }

  logout(){
    return this.http.post('http://localhost:8000/api/logout', {}, {withCredentials: true});
  }
}
