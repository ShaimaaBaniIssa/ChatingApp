import { HttpClient } from '@angular/common/http';
import { Injectable, Pipe } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment';
// making http request
// service lives for the lifetime of the app --> Singleton
@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  // to check from other components if the user logged in or not

  private currentUserSource = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe
      (
        map((response: User) => {
          const user = response;
          if (user) {
            this.setCurrentUser(user);
          }
          return user;
        })
      );
  }
  register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe
      (
        map(user => {
          if (user) {
            this.setCurrentUser(user);
          }
        })
      )
  }
  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }
  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}
