import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { map,Observable } from 'rxjs';
import { ProductDTO } from '../models';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  constructor(private http: HttpClient) { }

  private url = environment.apiUrl + "/v1/ProductApi";
  httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

  GetProductInCategory(categoryname: string) {
    return this.http.get(`${this.url}/GetProductInCategory?categoryName=${categoryname}`, this.httpOptions);
  }

  GetMostPopularProducts() {
    return this.http.get(this.url + '/GetMostPopularProducts', this.httpOptions);
  }
  
  Index() {
    return this.http.get(this.url + '/Index', this.httpOptions);
  } 

  ProductDetails(productId: string) {
    const browserId = localStorage.getItem("efne434wgmwbq1176")!;
    return this.http.get(this.url + `/ProductDetails?productId=${productId}&browserId=${browserId}`, this.httpOptions);
  }

  GetProductByCategoryName(categoryname: string): Observable<ProductDTO>{
    return this.http.get<ProductDTO>(`${this.url}/${'Get'}/${categoryname}`);
  }

  Bookmark(productId: string, userId: string): Observable<boolean>{
    return this.http.get<boolean>(`${this.url}/Bookmark?productId=${productId}&userId=${userId}`, this.httpOptions);
  }

  Search(productName: string): Observable<string> {
    return this.http.get<string>(`${this.url}/Search?searchText=${productName}`, this.httpOptions);
  }
}
