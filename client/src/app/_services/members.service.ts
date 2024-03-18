import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>;
  userParams: UserParams;

  constructor(private http: HttpClient) {
    this.userParams = new UserParams();

  }
  getUserParams() { return this.userParams; }
  setUserParams(userParams: UserParams) { this.userParams = userParams; }
  resetUserParams() {
    this.userParams = new UserParams();
    return this.userParams;
  }

  getMembers(userParams: UserParams) {
    const response = this.memberCache.get(Object.values(userParams).join('-'));
    if (response) {
      return of(response);
    }
    // require userParams as query
    // will recieve PaginationHeader and Members List in the body
    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
    if (userParams.gender) {
      params = params.append('gender', userParams.gender);
    }
    params = params.append('orderBy', userParams.orderBy);

    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http).pipe(
      map(
        response => {
          this.memberCache.set(Object.values(userParams).join('-'), response);
          return response;
        }
      )
    );
  }
  getMember(userName: string) {
    // array.reduce((result, current) => { ... }, initialValue)
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((u: Member) => u.userName == userName);
    if (member) return of(member);

    return this.http.get<Member>(this.baseUrl + 'users/' + userName);
  }
  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = { ...this.members[index], ...member }
      })
    );
  }
  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});

  }
  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }
  addLike(userName: string) {
    return this.http.post(this.baseUrl + 'likes/' + userName, {});
  }
  getUserLikes(predicate: string, pageNumber: number, pageSize: number) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);

    return getPaginatedResult<Member[]>(this.baseUrl + 'likes', params, this.http);
  }

}
