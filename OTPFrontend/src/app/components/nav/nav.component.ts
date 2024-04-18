import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css'
})
export class NavComponent implements OnInit{

  authenticated = false;

  constructor(private authService: AuthService, private router: Router) {

  }

  ngOnInit(): void {
    AuthService.behaviorSubject.subscribe(authenticated => {
      this.authenticated = authenticated;
      console.log(`This is what i got ${authenticated}`)
    })
    
  }

  logout() {
    AuthService.behaviorSubject.next(false);
    this.authService.logout().subscribe( () => {
      this.authService.accessToken = '';    
    })
  }

}
