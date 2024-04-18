import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit{
  message = "You ar not authenticated";

  constructor(private authService: AuthService) {

  }

  ngOnInit(): void {
    this.authService.user().subscribe({
      next: (res: any) => {
        this.message = `Hi ${res.first_name} ${res.last_name}`
        AuthService.behaviorSubject.next(true);
      },
      error: err => {
        this.message = `You ar not authenticated`
        AuthService.behaviorSubject.next(false);
        console.log("error")
      }
    });
  }

}
