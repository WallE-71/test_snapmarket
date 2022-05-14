import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { UserDto } from '../models';
// import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class SecurityService {
  constructor(private http: HttpClient) { }

  private url = environment.apiUrl + "/v1/AccountApi";
  httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

  RegisterOrSignIn(phoneNumber: string): Observable<UserDto> {
    const browserId = localStorage.getItem("efne434wgmwbq1176")!;
    return this.http.post<UserDto>(`${this.url}?phoneNumber=${phoneNumber}&browserId=${browserId}`, this.httpOptions);
  }

  ReciveCode(email: string) {
    return this.http.post(`${this.url}/SendCode?email=${email}`, this.httpOptions);
  }

  EditProfile(user: any) {
    const phoneNumber = this.GetFieldFromJWT('phoneNumber')!;
    return this.http.put(`${this.url}?phoneNumber=${phoneNumber}&email=${user.email}&firstName=${user.firstName}&lastName=${user.lastName}&address=${user.address}`, this.httpOptions);
  }

  GetCreditCart(phoneNumber: string):Observable<any>{
    return this.http.post<any>(`${this.url}/CreditCart?phoneNumber=${phoneNumber}&credit=&nationalId=&bankCode=&getCart=${true}`, this.httpOptions);
  }

  IncreaseCredit(phoneNumber: string, credit: number, nationalId: string, bankCode: string){
    return this.http.post(`${this.url}/CreditCart?phoneNumber=${phoneNumber}&credit=${credit}&nationalId=${nationalId}&bankCode=${bankCode}&getCart=${false}`, this.httpOptions);
  }

  SignOut() {
    localStorage.removeItem('efne434wgmwbq1176');
    localStorage.removeItem('tie1i1we1xn5');
    localStorage.removeItem('kwee2b1wguuo483d');
  }

  SaveData(response: any) {
    if (response.browserId != null)
      localStorage.setItem('efne434wgmwbq1176', response.browserId);
    if (response.token != null)
      localStorage.setItem('tie1i1we1xn5', response.token.token);
    if (response.token != null)
      localStorage.setItem('kwee2b1wguuo483d', response.token.expiration);
  }

  encode_utf8(s: any) {
    return unescape(encodeURIComponent(s));
  }

  decode_utf8(s: any) {
    return decodeURIComponent(escape(s));
  }

  GetFieldFromJWT(field: string): string {
    const token = localStorage.getItem('tie1i1we1xn5');
    if (!token) { return ''; }
    const dataToken = JSON.parse(atob(token.split('.')[1]));

    if (field != 'phoneNumber' && field != 'email')
      return this.decode_utf8(dataToken[field]);
    else
      return dataToken[field]
  }

  IsAuthenticated(): boolean {
    const token = localStorage.getItem('tie1i1we1xn5');
    if (!token) return false;

    const expiration = localStorage.getItem('kwee2b1wguuo483d')!;
    const expirationDate = new Date(expiration);
    if (expirationDate <= new Date()){
      this.SignOut();
      return false;
    }
    return true;
  }
}
