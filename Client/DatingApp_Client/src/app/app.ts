import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';


@Component({
  selector: 'app-root',
  imports: [],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  
  // Angular DI
  private http = inject(HttpClient)
  protected readonly title = signal('Dating App');
  protected members:any;

  ngOnInit(): void {
    this.http.get('https://localhost:5001/api/members/getallmembers').subscribe({
      next: response=> this.members = response,
      error: error=> console.log(error),
      complete: () => console.log('completed the Http Request..')
    })
  }  


  }
