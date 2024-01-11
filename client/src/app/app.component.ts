import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'Chating App';
  users: any;
  constructor(private http: HttpClient) {

  }
  ngOnInit(): void { // initialization tasks , after constructor
    this.http.get('https://localhost:5001/api/users').subscribe({
      next: (response) => { // what to do with response
        this.users = response
      },
      error: (error) => {console.log(error) },
      complete:()=>{console.log('Request has completed')}
    }); // return Observable , body as ArrayBuffer
  }

}
