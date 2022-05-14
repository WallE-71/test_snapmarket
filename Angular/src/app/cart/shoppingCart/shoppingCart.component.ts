import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { CartService } from '../../services/cart.service';
import { AppComponent } from '../../app.component';
import { CartDto, CartItemsDto } from '../../models';
import { UserComponent } from '../../security/user/user.component';
import { SecurityService } from '../../services/security.service';
import { Router } from '@angular/router';
import Swal from 'sweetalert2/dist/sweetalert2.js';

declare var $: any;

@Component({
  selector: 'app-shoppingCart',
  templateUrl: './shoppingCart.component.html',
  styleUrls: ['./shoppingCart.component.css'],
})
export class ShoppingCartComponent implements OnInit {
  constructor(private cartService: CartService, private securityService: SecurityService){}
  cart: CartDto = { productCount: 0, sumAmount: 0, data: this, cartItems: [] };
  status: number = 0;
  errors: string[] = [];
  cartItems: CartItemsDto[];
  private phoneNumber = this.securityService.GetFieldFromJWT('phoneNumber')!;
  
  LoadCart() {
    if (this.phoneNumber != null) {    
      this.cartService.Cart(this.phoneNumber).subscribe((result: any) => {
        if(result.data != null){
          this.cart = result.data;
          this.cartItems = result.data?.cartItems;
        }
      });
    }
    else {
      this.cart = { productCount: 0, sumAmount: 0, data: this.cart, cartItems: [] };
      this.cartItems = [];
    }
  }

  GetStatus() {
    const address = this.securityService.GetFieldFromJWT('address')!;
    if (this.phoneNumber! != "null" && address == "")
      this.status = 1;
    else if (this.phoneNumber! === null)
      this.status = 0;
    else if (address != "null")
      this.status = 2;
  }

  SaveItem(productId: string) {
    if (productId != null)
      this.cartService.AddToCart(productId).subscribe();
  }

  DeleteItem(productId: number) {
    this.cartService.Delete(productId).subscribe();
  }
  
  DeleteAll() {
    this.cartService.DeleteAll().subscribe();
  }
  
  RegisterOrLogin() {
    var user = new UserComponent(this.securityService);
    user.RegisterOrLogin(),
    user.ngOnInit();
  }

  ngOnInit(): void {
    this.LoadCart();
    this.GetStatus();
    if (this.status == 0) {
      this.cart = { productCount: 0, sumAmount: 0, data: this.cart, cartItems: [] };
      this.cartItems = [];
    }
  }
}
